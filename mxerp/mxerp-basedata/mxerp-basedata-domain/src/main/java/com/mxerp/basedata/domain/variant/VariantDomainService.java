package com.mxerp.basedata.domain.variant;

import com.mxerp.basedata.domain.material.Material;
import com.mxerp.basedata.domain.material.MaterialRepository;
import com.mxerp.basedata.domain.specattribute.AssignedSpecAttributeData;
import com.mxerp.basedata.domain.specattribute.SpecAttribute;
import com.mxerp.basedata.domain.specattribute.SpecAttributeRepository;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.ValueTextUtils;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * SKU 领域服务
 *
 * <p>规格属性的归属用「分类」表达：SKU 自己不存物料类型，需要校验规格属性时顺着 {@code materialId} 找到物料， 再取物料的 {@code
 * categoryId}。这样不会出现「SKU 说是成品、物料却是辅料」这种两张皮的数据。
 *
 * <p>分类下配的规格属性就是生成规格用的维度：每条规格属性的「规格属性值」做笛卡尔积，一个组合落一条 SKU， 组合摊成 {@code VariantAttributeValue} 行存在这条
 * SKU 上，再由这些行拼出规格编码；生成完之后不再有 「哪个规格属性算规格」这个开关——是规格就由组合决定。
 *
 * <p>描述性取值本来是另一套（平台层的 {@code Attribute} 定义 + {@code AttributeAssignment} 赋值行），
 * 这套已挪出本模块，规格聚合上的属性挂载随之停用：公开方法的 {@code attributes} 入参先留着但不生效， 恢复挂载时把注释里的 {@code resolveAttributes}
 * 与 {@code syncAttributes} 接回来即可。
 */
@NullMarked
public class VariantDomainService {

  /** 单个物料最多生成的 SKU 数量，防止取值配错时一次写入几十万行 */
  private static final int MAX_GENERATE_COUNT = 10000;

  /** SKU 仓储 */
  private final VariantRepository variantRepository;

  /** 物料仓储 */
  private final MaterialRepository materialRepository;

  /** 规格属性仓储 */
  private final SpecAttributeRepository specAttributeRepository;

  // 属性仓储：属性那套定义/赋值已挪到平台层 com.mxerp.system.domain.attribute，本模块不依赖
  // system-domain，挂载先停用，接回来时再放开下面两处。
  // /** 属性仓储 */
  // private final AttributeRepository attributeRepository;

  /**
   * 构造器注入
   *
   * @param variantRepository SKU 仓储
   * @param materialRepository 物料仓储
   * @param specAttributeRepository 规格属性仓储
   */
  public VariantDomainService(
      VariantRepository variantRepository,
      MaterialRepository materialRepository,
      SpecAttributeRepository specAttributeRepository) {
    this.variantRepository = variantRepository;
    this.materialRepository = materialRepository;
    this.specAttributeRepository = specAttributeRepository;
    // this.attributeRepository = attributeRepository;
  }

  // ==================== 公开：创建 / 修改 ====================

  /**
   * 创建 SKU（入参为 Id，不解析 Code）
   *
   * @param materialId MaterialId
   * @param opc OPC（可空）
   * @param specAttributes 规格属性值——这一条规格由哪组值构成（可空）
   * @param attributes 属性值（可空，属性挂载已停用，暂不生效）
   * @return SKU 聚合
   * @throws DomainException 物料不存在 / OPC 已存在 / 规格属性不存在 / 规格属性组合重复时抛出
   */
  public Variant create(
      long materialId,
      @Nullable String opc,
      @Nullable Map<String, Object> specAttributes,
      @Nullable Map<String, Object> attributes) {
    Material material = getExistMaterial(materialId);
    return buildVariant(
        materialId,
        material.getCode(),
        opc,
        resolveSpecAttributes(material.getCategoryId(), specAttributes));
  }

  /**
   * 导入生成（存在则修改，不存在则新建）
   *
   * @param materialId 物料Id
   * @param opc OPC（可空）
   * @param specAttributes 规格属性值（可空）
   * @param attributes 属性值（可空，属性挂载已停用，暂不生效）
   * @return SKU 聚合
   * @throws DomainException 物料不存在 / 规格属性不存在 / OPC 已存在时抛出
   */
  public Variant importGenerate(
      long materialId,
      @Nullable String opc,
      @Nullable Map<String, Object> specAttributes,
      @Nullable Map<String, Object> attributes) {
    Material material = getExistMaterial(materialId);
    // 1. 规格属性值落成行数据（代码必须在物料所属分类下有定义）
    List<AssignedSpecAttributeData> specAttributeRows =
        resolveSpecAttributes(material.getCategoryId(), specAttributes);

    // 2. 按规格编码查询，存在则修改，不存在则新建
    String code = Variant.buildCode(material.getCode(), specAttributeRows);
    Variant model = variantRepository.getByCode(code);

    if (model == null) {
      model = buildVariant(materialId, material.getCode(), opc, specAttributeRows);
    } else {
      modify(model, opc);
    }
    return model;
  }

  /**
   * 修改（按Id查询后修改）
   *
   * <p>规格属性值不可改：那组行就是这条规格的身份，换一组等于换一条 SKU，应该走删除 + 重新生成。
   *
   * @param id SKU Id
   * @param opc OPC（可空）
   * @param attributes 属性值（可空，属性挂载已停用，暂不生效）
   * @return SKU 聚合
   * @throws DomainException SKU 不存在 / 物料不存在时抛出
   */
  public Variant modify(long id, @Nullable String opc, @Nullable Map<String, Object> attributes) {
    Variant model = variantRepository.get(id);
    if (model == null) {
      throw new DomainException("物料Variant不存在!");
    }
    modify(model, opc);
    return model;
  }

  /**
   * 从 Material 生成 SKU（按分类下的规格属性做笛卡尔积）
   *
   * <p>每条规格属性的候选值就是它在规格属性上维护好的「规格属性值」；分类下没有规格属性时只生成一条没有规格属性值的 SKU——没有规格维度的物料天然落在这一支，不需要为它写分支。
   *
   * <p>规格属性值增删后重新生成，落在候选值之外的旧 SKU 标记删除，保证 SKU 集合始终等于当前配置的展开。
   *
   * @param materialId MaterialId
   * @return 需要持久化的聚合列表（含新增 + 已标记删除的）
   * @throws DomainException 物料不存在 / 规格属性没有维护规格属性值 / 组合数超上限时抛出
   */
  public List<Variant> createFromMaterial(long materialId) {
    // 1. 查物料与其分类下的规格属性
    Material material = getExistMaterial(materialId);
    List<SpecAttribute> specAttributes =
        specAttributeRepository.getListByCategoryId(material.getCategoryId());

    // 2. 查已有 SKU
    List<Variant> existing = variantRepository.getListByMaterialId(material.getId());

    // 3. 没有规格属性：只保证有一条没有规格属性值的 SKU
    if (specAttributes.isEmpty()) {
      return generateEmptyVariant(material, existing);
    }

    // 4. 逐个规格属性取候选值行，再做笛卡尔积
    Map<String, List<AssignedSpecAttributeData>> candidates = new LinkedHashMap<>();
    for (SpecAttribute specAttribute : specAttributes) {
      candidates.put(specAttribute.getCode(), candidateRows(specAttribute));
    }
    List<List<AssignedSpecAttributeData>> combinations = cartesian(candidates);
    if (combinations.size() > MAX_GENERATE_COUNT) {
      throw new DomainException(
          "规格属性组合共" + combinations.size() + "种，超过单次生成上限" + MAX_GENERATE_COUNT + "，请缩减规格属性取值!");
    }

    // 5. 生成候选值内的 SKU
    List<Variant> toSave = new ArrayList<>();
    for (List<AssignedSpecAttributeData> combination : combinations) {
      String code = Variant.buildCode(material.getCode(), combination);
      boolean exists = existing.stream().anyMatch(x -> code.equals(x.getCode()));
      if (!exists) {
        toSave.add(buildVariant(material.getId(), material.getCode(), null, combination));
      }
    }

    // 6. 已有 SKU 的取值若不在候选集合内，标记删除
    for (Variant variant : existing) {
      if (isOutOfRange(variant, candidates)) {
        variant.delete();
        toSave.add(variant);
      }
    }
    return toSave;
  }

  // ==================== 私有：创建 / 修改 ====================

  /**
   * 构建 SKU（含 OPC 唯一性、规格属性组合唯一性校验）
   *
   * @param materialId MaterialId
   * @param opc OPC（可空）
   * @param specAttributeRows 规格属性值行数据（已校验）
   * @return SKU
   * @throws DomainException OPC 已存在 / 规格属性组合重复时抛出
   */
  private Variant buildVariant(
      long materialId,
      String materialCode,
      @Nullable String opc,
      List<AssignedSpecAttributeData> specAttributeRows) {
    // 1. OPC 唯一性（可空）
    if (opc != null
        && !opc.isBlank()
        && variantRepository.isExistByMaterialIdAndOpcExcludeId(materialId, opc)) {
      throw new DomainException("OPC:" + opc + "已存在!");
    }

    // 2. 构造规格（规格属性值行在构造时写入）
    Variant model = new Variant(materialId, materialCode, opc, specAttributeRows);

    // 3. 规格编码唯一性
    if (variantRepository.isExistByCode(model.getCode())) {
      throw new DomainException("相同规格属性组合的规格已存在!");
    }

    return model;
  }

  /**
   * 修改（使用已有聚合，避免重复查询）
   *
   * @param model SKU 聚合
   * @param opc OPC（可空）
   * @throws DomainException OPC 已存在 / 物料不存在时抛出
   */
  private void modify(Variant model, @Nullable String opc) {
    // 1. OPC 唯一性（排除自身）
    if (opc != null
        && !opc.isBlank()
        && variantRepository.isExistByMaterialIdAndOpcExcludeId(
            model.getMaterialId(), opc, model.getId())) {
      throw new DomainException("OPC:" + opc + "已存在!");
    }

    // 2. 修改（只动可变字段，规格属性值行保持不变）
    model.modify(opc);
  }

  // ==================== 私有：规格属性值解析 ====================

  /**
   * 解析规格属性值入参：按代码在物料所属分类下找定义，摊平成「一个取值一行」的行数据
   *
   * <p>代码对不上就当场报错：编码是按属性代码拼的，留下一个分类下没有的代码，重新生成时这条 SKU 会被当成「取值已失效」删掉。
   *
   * <p>取值存维护好的原样文本，不做按 {@code DataType} 的类型换算——数字规范只在拼编码时做， 否则 {@code 0755} 这种当编码用的取值会被削成 {@code
   * 755}。
   *
   * @param categoryId 物料分类Id
   * @param specAttributes 规格属性值（可空，一个值可以是标量或数组）
   * @return 规格属性值行数据，没有给值时返回空列表
   * @throws DomainException 规格属性不存在时抛出
   */
  private List<AssignedSpecAttributeData> resolveSpecAttributes(
      long categoryId, @Nullable Map<String, Object> specAttributes) {
    Map<String, List<String>> flattened = ValueTextUtils.flatten(specAttributes);
    if (flattened.isEmpty()) {
      return List.of();
    }
    Map<String, SpecAttribute> dict = new TreeMap<>();
    for (SpecAttribute definition : specAttributeRepository.getListByCategoryId(categoryId)) {
      dict.put(definition.getCode(), definition);
    }

    List<AssignedSpecAttributeData> rows = new ArrayList<>();
    for (Map.Entry<String, List<String>> entry : flattened.entrySet()) {
      SpecAttribute definition = dict.get(entry.getKey());
      if (definition == null) {
        throw new DomainException("规格属性" + entry.getKey() + "不存在!");
      }
      for (String value : entry.getValue()) {
        rows.add(new AssignedSpecAttributeData(definition.getId(), definition.getCode(), value));
      }
    }
    return rows;
  }

  /**
   * 取一个规格属性的候选值，直接落成规格属性值行数据
   *
   * <p>候选值只有这一个来源：规格属性上维护好的「规格属性值」。没有值就是没有可生成的规格，不做任何按名字猜区间的
   * 兜底——否则规格属性上多一个键就多出一种隐式语义，查问题时根本看不出来它是谁。
   *
   * @param specAttribute 规格属性
   * @return 规格属性值行数据（同一代码的每个候选值一行）
   * @throws DomainException 规格属性没有维护规格属性值时抛出
   */
  private List<AssignedSpecAttributeData> candidateRows(SpecAttribute specAttribute) {
    List<AssignedSpecAttributeData> rows =
        specAttribute.getSpecAttributeValues().stream()
            .filter(value -> !value.getIsDeleted())
            .map(
                value ->
                    new AssignedSpecAttributeData(
                        specAttribute.getId(), specAttribute.getCode(), value.getValue()))
            .toList();
    if (rows.isEmpty()) {
      throw new DomainException("规格属性" + specAttribute.getCode() + "没有维护规格属性值，无法生成SKU!");
    }
    return rows;
  }

  // ==================== 私有：从 Material 生成 SKU ====================

  /**
   * 无规格属性分支：已有空规格属性 SKU 则不动，没有则生成一条
   *
   * @param material 物料
   * @param existing 已有 SKU 列表
   * @return 待持久化列表
   */
  private List<Variant> generateEmptyVariant(Material material, List<Variant> existing) {
    List<Variant> toSave = new ArrayList<>();
    String code = Variant.buildCode(material.getCode(), null);
    boolean exists = existing.stream().anyMatch(x -> code.equals(x.getCode()));
    if (!exists) {
      toSave.add(buildVariant(material.getId(), material.getCode(), null, List.of()));
    }
    return toSave;
  }

  /**
   * 规格属性组合的笛卡尔积，结果按代码排序保证编码稳定
   *
   * @param candidates 每个规格属性的候选值行
   * @return 组合列表，一个组合就是一组规格属性值行
   */
  private List<List<AssignedSpecAttributeData>> cartesian(
      Map<String, List<AssignedSpecAttributeData>> candidates) {
    List<List<AssignedSpecAttributeData>> result = new ArrayList<>();
    result.add(List.of());
    for (Map.Entry<String, List<AssignedSpecAttributeData>> entry : candidates.entrySet()) {
      List<List<AssignedSpecAttributeData>> expanded = new ArrayList<>();
      for (List<AssignedSpecAttributeData> combination : result) {
        for (AssignedSpecAttributeData row : entry.getValue()) {
          List<AssignedSpecAttributeData> next = new ArrayList<>(combination);
          next.add(row);
          expanded.add(next);
        }
      }
      result = expanded;
    }
    return result;
  }

  /**
   * 判断已有 SKU 的取值是否落在候选集合之外
   *
   * @param variant 已有 SKU
   * @param candidates 每个规格属性的候选值行
   * @return true：某个规格属性在这一条规格上没有值，或值已失效
   */
  private boolean isOutOfRange(
      Variant variant, Map<String, List<AssignedSpecAttributeData>> candidates) {
    Map<String, List<String>> filled =
        variant.getSpecAttributes().stream()
            .collect(
                Collectors.groupingBy(
                    VariantSpecAttributeValue::getCode,
                    Collectors.mapping(VariantSpecAttributeValue::getValue, Collectors.toList())));

    for (Map.Entry<String, List<AssignedSpecAttributeData>> entry : candidates.entrySet()) {
      List<String> actual = filled.get(entry.getKey());
      if (actual == null || actual.isEmpty()) {
        return true;
      }
      List<String> allowed =
          entry.getValue().stream().map(row -> ValueTextUtils.canonicalText(row.value())).toList();
      for (String value : actual) {
        if (!allowed.contains(ValueTextUtils.canonicalText(value))) {
          return true;
        }
      }
    }
    return false;
  }

  // ==================== 私有：取数 ====================

  /**
   * 取物料，不存在则抛异常
   *
   * @param materialId MaterialId
   * @return 物料聚合
   * @throws DomainException 物料不存在时抛出
   */
  private Material getExistMaterial(long materialId) {
    Material material = materialRepository.get(materialId);
    if (material == null) {
      throw new DomainException("物料" + materialId + "不存在!");
    }
    return material;
  }

  // ==================== 私有：属性值解析（已停用） ====================

  // /**
  //  * 按规格这一持有者类型下的属性定义解析属性值
  //  *
  //  * <p>定义不按分类配：属性说的是「规格这类对象能挂什么描述」，跟物料归哪一类无关。
  //  *
  //  * <p>入参为 null 时返回 null，聚合同步会跳过这一步——只带规格组合的导入不能顺手把已维护的属性值清空。
  //  *
  //  * @param attributes 属性值（可空）
  //  * @return 属性值行数据（可空）
  //  * @throws DomainException 属性没有定义、取值不合法或必填未给值时抛出
  //  */
  // @Nullable
  // private List<AssignedAttributeData> resolveAttributes(
  //     @Nullable Map<String, Object> attributes) {
  //   if (attributes == null) {
  //     return null;
  //   }
  //   return AttributeUtils.resolve(
  //       attributeRepository.getListByEntityType("Variant"),
  //       attributes);
  // }
}
