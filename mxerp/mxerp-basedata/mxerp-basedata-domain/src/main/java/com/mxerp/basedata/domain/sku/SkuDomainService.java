package com.mxerp.basedata.domain.sku;

import com.mxerp.basedata.domain.attribute.Attribute;
import com.mxerp.basedata.domain.attribute.AttributeRepository;
import com.mxerp.basedata.domain.attribute.AttributeScopeEnum;
import com.mxerp.basedata.domain.spu.Spu;
import com.mxerp.basedata.domain.spu.SpuRepository;
import com.mxerp.shared.domain.enums.ProductTypeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.util.*;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** SKU 领域服务 */
@NullMarked
public class SkuDomainService {

  /** SKU 仓储 */
  private final SkuRepository skuRepository;

  /** 产品仓储 */
  private final SpuRepository spuRepository;

  /** 属性仓储 */
  private final AttributeRepository attributeRepository;

  /**
   * 构造器注入
   *
   * @param skuRepository SKU 仓储
   * @param spuRepository 产品仓储
   * @param attributeRepository 属性仓储
   */
  public SkuDomainService(
      SkuRepository skuRepository,
      SpuRepository spuRepository,
      AttributeRepository attributeRepository) {
    this.skuRepository = skuRepository;
    this.spuRepository = spuRepository;
    this.attributeRepository = attributeRepository;
  }

  // ==================== 公开：创建 / 修改 ====================

  /**
   * 创建 SKU（入参为 Id，不解析 Code）
   *
   * @param productType 产品类型
   * @param spuId SpuId
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param attributes 属性值（可空）
   * @return SKU 聚合
   * @throws DomainException 产品不存在 / OPC 已存在 / 属性不存在 / 属性组合重复时抛出
   */
  public Sku create(
      String productType,
      long spuId,
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> attributes) {
    SplitAttributes split = splitAttributes(productType, attributes);
    return buildSku(
        productType,
        spuId,
        opc,
        purchasePrice,
        currencyId,
        unitId,
        taxRateId,
        moq,
        split.key(),
        split.other());
  }

  /**
   * 导入生成（存在则修改，不存在则新建）
   *
   * @param productType 产品类型
   * @param spuId 产品Id
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param attributes 属性值（可空）
   * @return SKU 聚合
   * @throws DomainException 属性不存在 / OPC 已存在时抛出
   */
  public Sku importGenerate(
      String productType,
      long spuId,
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> attributes) {
    // 1. 拆分属性
    SplitAttributes split = splitAttributes(productType, attributes);

    // 2. 按特征值查询，存在则修改，不存在则新建
    String signature = Sku.calculateSignature(spuId, split.key());
    Sku model = skuRepository.getByAttributesSignature(signature);

    if (model == null) {
      model =
          buildSku(
              productType,
              spuId,
              opc,
              purchasePrice,
              currencyId,
              unitId,
              taxRateId,
              moq,
              split.key(),
              split.other());
    } else {
      modify(model, opc, purchasePrice, currencyId, unitId, taxRateId, moq, attributes);
    }
    return model;
  }

  /**
   * 修改（按Id查询后修改）
   *
   * @param id SKU Id
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param attributes 属性值（可空）
   * @return SKU 聚合
   * @throws DomainException SKU 不存在时抛出
   */
  public Sku modify(
      long id,
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> attributes) {
    Sku model = skuRepository.get(id);
    if (model == null) {
      throw new DomainException("产品Sku不存在!");
    }
    modify(model, opc, purchasePrice, currencyId, unitId, taxRateId, moq, attributes);
    return model;
  }

  /**
   * 从 Spu 生成 SKU（按产品类型分流）
   *
   * <p>辅料：生成一个无关键属性的 SKU。
   *
   * <p>成品：按 SPH / CYL 组合生成；超出新区间的旧 SKU 标记删除。
   *
   * @param spuId SpuId
   * @return 需要持久化的聚合列表（含新增 + 已标记删除的）
   * @throws DomainException 产品不存在 / 属性缺失时抛出
   */
  public List<Sku> createFromSpu(long spuId) {
    // 1. 查产品
    Spu spu = spuRepository.get(spuId);
    if (spu == null) {
      throw new DomainException("产品" + spuId + "不存在!");
    }

    // 2. 查已有 SKU
    List<Sku> existing = skuRepository.getListBySpuId(spu.getId());
    List<Sku> toSave = new ArrayList<>();

    String productType = spu.getProductType();

    // 3. 按产品类型分流
    if (ProductTypeEnum.ACCESSORY.getCode().equals(productType)) {
      createForAccessory(spu, existing, toSave);
    } else if (ProductTypeEnum.END_PRODUCT.getCode().equals(productType)) {
      createForEndProduct(spu, existing, toSave);
    }

    return toSave;
  }

  // ==================== 私有：创建 / 修改 ====================

  /**
   * 构建 SKU（含产品存在性、OPC 唯一性、属性组合唯一性校验）
   *
   * @param productType 产品类型
   * @param spuId SpuId
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param keyAttributes 关键属性（已拆分，可空）
   * @param otherAttributes 非关键属性（已拆分，可空）
   * @return SKU
   * @throws DomainException 产品不存在 / OPC 已存在 / 属性组合重复时抛出
   */
  private Sku buildSku(
      String productType,
      long spuId,
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> keyAttributes,
      @Nullable TreeMap<String, Object> otherAttributes) {
    // 1. 校验产品存在
    Spu spu = spuRepository.get(spuId);
    if (spu == null) {
      throw new DomainException("产品" + spuId + "不存在!");
    }

    // 2. OPC 唯一性（可空）
    if (opc != null && !opc.isBlank() && skuRepository.isExistBySpuIdAndOpcExcludeId(spuId, opc)) {
      throw new DomainException("OPC:" + opc + "已存在!");
    }

    // 3. 构造 SKU
    Sku model =
        new Sku(
            productType,
            spuId,
            opc,
            purchasePrice,
            currencyId,
            unitId,
            taxRateId,
            moq,
            keyAttributes,
            otherAttributes);

    // 4. 属性签名唯一性
    if (skuRepository.isExistByAttributesSignature(model.getAttributesSignature())) {
      throw new DomainException("相同属性组合的Sku已存在!");
    }

    return model;
  }

  /**
   * 修改（使用已有聚合，避免重复查询）
   *
   * @param model SKU 聚合
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param attributes 属性值（可空）
   * @throws DomainException OPC 已存在时抛出
   */
  private void modify(
      Sku model,
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> attributes) {
    // 1. OPC 唯一性（排除自身）
    if (opc != null
        && !opc.isBlank()
        && skuRepository.isExistBySpuIdAndOpcExcludeId(model.getSpuId(), opc, model.getId())) {
      throw new DomainException("OPC:" + opc + "已存在!");
    }

    // 2. 只保留非关键属性（关键属性改了会变签名）
    TreeMap<String, Object> otherAttributes =
        filterOtherAttributes(model.getProductType(), attributes);

    // 3. 修改
    model.modify(opc, purchasePrice, currencyId, unitId, taxRateId, moq, otherAttributes);
  }

  // ==================== 私有：属性处理 ====================

  /**
   * 拆分属性为「关键 / 非关键」
   *
   * @param productType 产品类型
   * @param attributes 属性值（可空）
   * @return 拆分结果
   * @throws DomainException 属性不存在时抛出
   */
  private SplitAttributes splitAttributes(
      String productType, @Nullable TreeMap<String, Object> attributes) {
    TreeMap<String, Object> key = new TreeMap<>();
    TreeMap<String, Object> other = new TreeMap<>();
    if (attributes != null) {
      for (Map.Entry<String, Object> entry : attributes.entrySet()) {
        Attribute attr = getAttribute(productType, entry.getKey());
        if (attr.getIsKey()) {
          key.put(entry.getKey(), entry.getValue());
        } else {
          other.put(entry.getKey(), entry.getValue());
        }
      }
    }
    return new SplitAttributes(key, other);
  }

  /**
   * 只保留非关键属性（关键属性改了会变签名）
   *
   * @param productType 产品类型
   * @param attributes 属性值（可空）
   * @return 非关键属性 Map
   * @throws DomainException 属性不存在时抛出
   */
  private TreeMap<String, Object> filterOtherAttributes(
      String productType, @Nullable TreeMap<String, Object> attributes) {
    TreeMap<String, Object> other = new TreeMap<>();
    if (attributes != null) {
      for (Map.Entry<String, Object> entry : attributes.entrySet()) {
        Attribute attr = getAttribute(productType, entry.getKey());
        if (!attr.getIsKey()) {
          other.put(entry.getKey(), entry.getValue());
        }
      }
    }
    return other;
  }

  /**
   * 获取属性（按产品类型 + SKU 范围 + 属性代码）
   *
   * @param productType 产品类型
   * @param attributeCode 属性代码
   * @return 属性
   * @throws DomainException 属性不存在时抛出
   */
  private Attribute getAttribute(String productType, String attributeCode) {
    Attribute attribute =
        attributeRepository.getByProductTypeAndScopeAndCode(
            productType, AttributeScopeEnum.SKU.getCode(), attributeCode);
    if (attribute == null) {
      throw new DomainException("属性" + attributeCode + ":不存在!");
    }
    return attribute;
  }

  // ==================== 私有：从 Spu 生成 SKU ====================

  /**
   * 辅料：生成一个无关键属性的 SKU（如已存在则跳过）
   *
   * @param spu 产品
   * @param existing 已有 SKU 列表（用于去重）
   * @param toSave 待持久化列表（新增的 SKU 会加进去）
   */
  private void createForAccessory(Spu spu, List<Sku> existing, List<Sku> toSave) {
    String signature = Sku.calculateSignature(spu.getId(), null);
    boolean exists = existing.stream().anyMatch(x -> signature.equals(x.getAttributesSignature()));
    if (!exists) {
      Sku model =
          buildSku(
              spu.getProductType(), spu.getId(), null, null, null, null, null, null, null, null);
      toSave.add(model);
    }
  }

  /**
   * 成品：按 SPH / CYL 组合生成，超出区间的旧 SKU 标记删除
   *
   * @param spu 产品
   * @param existing 已有 SKU 列表（用于去重和对比区间）
   * @param toSave 待持久化列表（新增的和被标记删除的 SKU 都会加进去）
   * @throws DomainException 产品属性 MinSPH / MaxSPH / MinCYL / MaxCYL 缺失时抛出
   */
  private void createForEndProduct(Spu spu, List<Sku> existing, List<Sku> toSave) {
    int minSph = getIntAttr(spu, "MinSPH");
    int maxSph = getIntAttr(spu, "MaxSPH");
    int minCyl = getIntAttr(spu, "MinCYL");
    int maxCyl = getIntAttr(spu, "MaxCYL");

    // 1. 生成新区间内的 SKU（已存在的跳过）
    for (int sph = minSph; sph <= maxSph; sph += 25) {
      for (int cyl = minCyl; cyl <= maxCyl; cyl += 25) {
        TreeMap<String, Object> dict = new TreeMap<>();
        dict.put("SPH", sph);
        dict.put("CYL", cyl);
        String signature = Sku.calculateSignature(spu.getId(), dict);
        boolean exists =
            existing.stream().anyMatch(x -> signature.equals(x.getAttributesSignature()));
        if (!exists) {
          Sku model =
              buildSku(
                  spu.getProductType(),
                  spu.getId(),
                  null,
                  null,
                  null,
                  null,
                  null,
                  null,
                  dict,
                  null);
          toSave.add(model);
        }
      }
    }

    // 2. 计算已有 SKU 的 SPH / CYL 区间
    int skuMinSph = existing.isEmpty() ? 0 : minKeyAttr(existing, "SPH");
    int skuMaxSph = existing.isEmpty() ? 0 : maxKeyAttr(existing, "SPH");
    int skuMinCyl = existing.isEmpty() ? 0 : minKeyAttr(existing, "CYL");
    int skuMaxCyl = existing.isEmpty() ? 0 : maxKeyAttr(existing, "CYL");

    // 3. 四个方向：超出新区间的旧 SKU 标记删除
    if (minSph > skuMinSph) {
      for (int sph = skuMinSph; sph < minSph; sph += 25) {
        for (int cyl = skuMinCyl; cyl <= skuMaxCyl; cyl += 25) {
          removeByKey(existing, spu.getId(), sph, cyl, toSave);
        }
      }
    }
    if (maxSph < skuMaxSph) {
      for (int sph = skuMaxSph; sph > maxSph; sph -= 25) {
        for (int cyl = skuMinCyl; cyl <= skuMaxCyl; cyl += 25) {
          removeByKey(existing, spu.getId(), sph, cyl, toSave);
        }
      }
    }
    if (minCyl > skuMinCyl) {
      for (int sph = skuMinSph; sph <= skuMaxSph; sph += 25) {
        for (int cyl = skuMinCyl; cyl < minCyl; cyl += 25) {
          removeByKey(existing, spu.getId(), sph, cyl, toSave);
        }
      }
    }
    if (maxCyl < skuMaxCyl) {
      for (int sph = skuMinSph; sph <= skuMaxSph; sph += 25) {
        for (int cyl = skuMaxCyl; cyl > maxCyl; cyl -= 25) {
          removeByKey(existing, spu.getId(), sph, cyl, toSave);
        }
      }
    }
  }

  /**
   * 按 SPH / CYL 找到对应 SKU，标记删除并加入待持久化列表
   *
   * @param existing 已有 SKU 列表
   * @param spuId SpuId
   * @param sph 球镜度
   * @param cyl 柱镜度
   * @param toSave 待持久化列表（标记删除的 SKU 会加进去）
   */
  private void removeByKey(List<Sku> existing, long spuId, int sph, int cyl, List<Sku> toSave) {
    TreeMap<String, Object> dict = new TreeMap<>();
    dict.put("SPH", sph);
    dict.put("CYL", cyl);
    String signature = Sku.calculateSignature(spuId, dict);
    Sku model =
        existing.stream()
            .filter(x -> signature.equals(x.getAttributesSignature()))
            .findFirst()
            .orElse(null);
    if (model != null) {
      model.delete();
      toSave.add(model);
    }
  }

  /**
   * 读取 Spu 属性里的整数值，缺失时抛异常
   *
   * @param spu 产品
   * @param key 属性键（如 MinSPH）
   * @return 属性整数值
   * @throws DomainException 属性缺失或格式错误时抛出
   */
  private Integer getIntAttr(Spu spu, String key) {
    Object value = spu.getAttributes() == null ? null : spu.getAttributes().get(key);
    if (value == null) {
      throw new DomainException("产品属性[" + key + "]不存在!");
    }
    return Integer.parseInt(value.toString());
  }

  /**
   * 读取 Sku 关键属性里的整数最小值
   *
   * @param skus SKU 列表
   * @param key 属性键（如 SPH）
   * @return 最小值，列表为空时返回 0
   */
  private Integer minKeyAttr(List<Sku> skus, String key) {
    return skus.stream().mapToInt(s -> getKeyAttrInt(s, key)).min().orElse(0);
  }

  /**
   * 读取 Sku 关键属性里的整数最大值
   *
   * @param skus SKU 列表
   * @param key 属性键（如 SPH）
   * @return 最大值，列表为空时返回 0
   */
  private Integer maxKeyAttr(List<Sku> skus, String key) {
    return skus.stream().mapToInt(s -> getKeyAttrInt(s, key)).max().orElse(0);
  }

  /**
   * 读取 Sku 关键属性里的整数值，缺失返回 0
   *
   * @param sku SKU
   * @param key 属性键（如 SPH）
   * @return 属性整数值，缺失或格式错误时返回 0
   */
  private Integer getKeyAttrInt(Sku sku, String key) {
    Object value = sku.getKeyAttributes() == null ? null : sku.getKeyAttributes().get(key);
    return value == null ? 0 : Integer.parseInt(value.toString());
  }

  // ==================== 内部：拆分结果 ====================

  /**
   * 属性拆分结果
   *
   * @param key 关键属性
   * @param other 非关键属性
   */
  private record SplitAttributes(TreeMap<String, Object> key, TreeMap<String, Object> other) {}
}
