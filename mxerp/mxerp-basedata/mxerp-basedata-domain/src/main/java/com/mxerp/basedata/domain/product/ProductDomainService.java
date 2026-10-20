package com.mxerp.basedata.domain.product;

import com.mxerp.basedata.domain.category.CategoryRepository;
import com.mxerp.shared.domain.exception.DomainException;
import java.util.List;
import java.util.Map;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 产品领域服务
 *
 * <p>属性那套（Attribute 定义 / AttributeAssignment 赋值）已挪到平台层
 * {@code com.mxerp.system.domain.attribute}；basedata-domain 不依赖 system-domain，产品聚合上的属性
 * 挂载暂时停用，本类里的属性校验与同步一并注释保留，接回来时恢复即可。公开的 {@code attributes} 入参
 * 先留着不生效，避免改签名牵连调用方。
 *
 * <p>校验只看属性定义，不要求代码同时是「参与规格生成的规格属性」（{@code ProductAttribute}）：两套定义各有
 * 用途，若在保存产品时强制查规格属性表，想在产品上记一个「重量」都得先去分类下建规格属性。
 */
@NullMarked
public class ProductDomainService {

  /** 产品仓储 */
  private final ProductRepository productRepository;

  /** 分类仓储 */
  private final CategoryRepository categoryRepository;

  // 属性仓储随属性挂载停用
  // /** 属性仓储 */
  // private final AttributeRepository attributeRepository;

  /**
   * 构造器注入
   *
   * @param productRepository 产品仓储
   * @param categoryRepository 分类仓储
   */
  public ProductDomainService(
      ProductRepository productRepository, CategoryRepository categoryRepository) {
    this.productRepository = productRepository;
    this.categoryRepository = categoryRepository;
    // this.attributeRepository = attributeRepository;
  }

  // ==================== 公开：创建 ====================

  /**
   * 创建产品
   *
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空，为 null 表示不改动）
   * @param attachmentIds 附件Id集合（可空）
   * @return 产品聚合
   * @throws DomainException 分类不存在 / 产品代码已存在 / 属性没有定义或取值不合法时抛出
   */
  public Product create(
      long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      long unitId,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark,
      @Nullable Map<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    // 1. 分类存在性
    validateCategory(categoryId);
    // 2. 代码唯一性
    if (productRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("产品代码" + code + "已存在!");
    }
    // 3. 属性值按产品这一类型下的定义校验并摊平成行数据（随属性挂载停用）
    // List<AssignedAttributeData> attributeRows = resolveAttributes(attributes);
    // 4. 构造聚合并同步属性值行
    Product product =
        new Product(
            categoryId,
            procurementType,
            supplierId,
            brandId,
            code,
            name,
            description,
            unitId,
            taxRateId,
            disabled,
            remark,
            attachmentIds);
    // product.syncAttributes(attributeRows);
    return product;
  }

  // ==================== 公开：导入 / 修改 ====================

  /**
   * 导入生成产品（存在则修改，不存在则新建）
   *
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空，为 null 表示不改动）
   * @return 产品聚合
   * @throws DomainException 分类不存在 / 产品代码已存在 / 属性校验不通过时抛出
   */
  public Product importGenerate(
      long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      long unitId,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark,
      @Nullable Map<String, Object> attributes) {
    // 存在则修改，不存在则新建
    Product model = productRepository.getByCode(code);
    if (model == null) {
      model =
          create(
              categoryId,
              procurementType,
              supplierId,
              brandId,
              code,
              name,
              description,
              unitId,
              taxRateId,
              disabled,
              remark,
              attributes,
              null);
    } else {
      modify(
          model,
          categoryId,
          procurementType,
          supplierId,
          brandId,
          code,
          name,
          description,
          unitId,
          taxRateId,
          disabled,
          remark,
          attributes,
          null);
    }
    return model;
  }

  /**
   * 修改产品（按Id查询后修改）
   *
   * @param id 产品Id
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空，为 null 表示不改动）
   * @param attachmentIds 附件Id集合（可空）
   * @return 产品聚合
   * @throws DomainException 产品不存在 / 分类不存在 / 产品代码已存在时抛出
   */
  public Product modify(
      long id,
      long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      long unitId,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark,
      @Nullable Map<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    // 1. 查产品
    Product model = productRepository.get(id);
    if (model == null) {
      throw new DomainException("产品不存在!");
    }
    // 2. 委托给使用已有聚合的重载
    modify(
        model,
        categoryId,
        procurementType,
        supplierId,
        brandId,
        code,
        name,
        description,
        unitId,
        taxRateId,
        disabled,
        remark,
        attributes,
        attachmentIds);
    return model;
  }

  // ==================== 公开：修改（已有聚合） ====================

  /**
   * 修改产品（使用已有聚合，避免重复查询）
   *
   * @param product 产品聚合
   * @param categoryId 所属分类Id
   * @param procurementType 取得方式
   * @param supplierId 供应商Id（可空）
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空，为 null 表示不改动）
   * @param attachmentIds 附件Id集合（可空）
   * @throws DomainException 分类不存在 / 产品代码已存在 / 属性校验不通过时抛出
   */
  public void modify(
      Product product,
      long categoryId,
      String procurementType,
      @Nullable Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      long unitId,
      @Nullable Long taxRateId,
      boolean disabled,
      @Nullable String remark,
      @Nullable Map<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    // 1. 分类存在性
    validateCategory(categoryId);
    // 2. 代码唯一性（排除自身）
    if (productRepository.isExistByCodeExcludeId(code, product.getId())) {
      throw new DomainException("产品代码" + code + "已存在!");
    }
    // 3. 属性值按产品这一类型下的定义校验（随属性挂载停用）
    // List<AssignedAttributeData> attributeRows = resolveAttributes(attributes);
    // 4. 修改主字段
    product.modify(
        categoryId,
        procurementType,
        supplierId,
        brandId,
        code,
        name,
        description,
        unitId,
        taxRateId,
        disabled,
        remark,
        attachmentIds);
    // 5. 同步属性值行（按「代码 + 值」匹配，输入里没有的行标记删除）
    // product.syncAttributes(attributeRows);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验分类存在
   *
   * @param categoryId 分类Id
   * @throws DomainException 分类不存在时抛出
   */
  private void validateCategory(long categoryId) {
    if (categoryRepository.get(categoryId) == null) {
      throw new DomainException("产品分类" + categoryId + "不存在!");
    }
  }

  // 属性值解析随属性挂载停用；接回来时按 Product 这一持有者类型取定义再解析
  // /**
  //  * 按产品这一持有者类型下的属性定义解析属性值
  //  *
  //  * <p>定义不按分类配：属性说的是「产品这类对象能挂什么描述」，跟物料归哪一类无关，否则同一个「重量」
  //  * 要在每个分类下重复配一遍。
  //  *
  //  * <p>入参为 null 时返回 null（聚合同步会跳过），传空 Map 才是清空，避免只改主字段的保存把已维护的
  //  * 属性值全删掉。
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
  //       attributeRepository.getListByEntityType("Product"),
  //       attributes);
  // }
}
