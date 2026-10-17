package com.mxerp.basedata.domain.spu;

import com.mxerp.basedata.domain.attribute.AttributeRepository;
import com.mxerp.basedata.domain.attribute.AttributeScopeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import java.math.BigDecimal;
import java.util.List;
import java.util.TreeMap;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品领域服务 */
@NullMarked
public class SpuDomainService {

  /** 产品仓储 */
  private final SpuRepository spuRepository;

  /** 属性仓储 */
  private final AttributeRepository attributeRepository;

  /**
   * 构造器注入
   *
   * @param spuRepository 产品仓储
   * @param attributeRepository 属性仓储
   */
  public SpuDomainService(SpuRepository spuRepository, AttributeRepository attributeRepository) {
    this.spuRepository = spuRepository;
    this.attributeRepository = attributeRepository;
  }

  // ==================== 公开：创建 ====================

  /**
   * 创建产品
   *
   * @param productType 产品类型
   * @param supplierId 供应商Id
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param purchasePrice 采购价格
   * @param currencyId 币种Id
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空）
   * @param attachmentIds 附件Id集合（可空）
   * @return 产品聚合
   * @throws DomainException 产品代码已存在 / 属性不存在时抛出
   */
  public Spu create(
      String productType,
      long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      long currencyId,
      long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    // 1. 代码唯一性
    if (spuRepository.isExistByCodeExcludeId(code)) {
      throw new DomainException("产品代码" + code + "已存在!");
    }
    // 2. 属性存在性
    validateAttributes(productType, attributes);
    // 3. 构造
    return new Spu(
        productType,
        supplierId,
        brandId,
        code,
        name,
        description,
        purchasePrice,
        currencyId,
        unitId,
        taxRateId,
        moq,
        disabled,
        remark,
        attributes,
        attachmentIds);
  }

  // ==================== 公开：导入 / 修改 ====================

  /**
   * 导入生成产品（存在则修改，不存在则新建）
   *
   * @param productType 产品类型
   * @param supplierId 供应商Id
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param purchasePrice 采购价格
   * @param currencyId 币种Id
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空）
   * @return 产品聚合
   * @throws DomainException 产品代码已存在 / 属性不存在时抛出
   */
  public Spu importGenerate(
      String productType,
      long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      long currencyId,
      long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes) {
    // 存在则修改，不存在则新建
    Spu model = spuRepository.getByProductTypeAndCode(productType, code);
    if (model == null) {
      model =
          create(
              productType,
              supplierId,
              brandId,
              code,
              name,
              description,
              purchasePrice,
              currencyId,
              unitId,
              taxRateId,
              moq,
              disabled,
              remark,
              attributes,
              null);
    } else {
      modify(
          model,
          supplierId,
          brandId,
          code,
          name,
          description,
          purchasePrice,
          currencyId,
          unitId,
          taxRateId,
          moq,
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
   * @param supplierId 供应商Id
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param purchasePrice 采购价格
   * @param currencyId 币种Id
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空）
   * @param attachmentIds 附件Id集合（可空）
   * @return 产品聚合
   * @throws DomainException 产品不存在时抛出
   * @throws DomainException 产品代码已存在时抛出
   */
  public Spu modify(
      long id,
      long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      long currencyId,
      long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    // 1. 查产品
    Spu model = spuRepository.get(id);
    if (model == null) {
      throw new DomainException("产品不存在!");
    }
    // 2. 委托给使用已有聚合的重载
    modify(
        model,
        supplierId,
        brandId,
        code,
        name,
        description,
        purchasePrice,
        currencyId,
        unitId,
        taxRateId,
        moq,
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
   * @param spu 产品聚合
   * @param supplierId 供应商Id
   * @param brandId 品牌Id（可空）
   * @param code 产品代码
   * @param name 产品名称
   * @param description 产品描述（可空）
   * @param purchasePrice 采购价格
   * @param currencyId 币种Id
   * @param unitId 单位Id
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param disabled 是否禁用
   * @param remark 备注（可空）
   * @param attributes 属性值（可空）
   * @param attachmentIds 附件Id集合（可空）
   * @throws DomainException 产品代码已存在 / 属性不存在时抛出
   */
  public void modify(
      Spu spu,
      long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      long currencyId,
      long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    // 1. 代码唯一性（排除自身）
    if (spuRepository.isExistByCodeExcludeId(code, spu.getId())) {
      throw new DomainException("产品代码" + code + "已存在!");
    }
    // 2. 属性存在性
    validateAttributes(spu.getProductType(), attributes);
    // 3. 修改
    spu.modify(
        supplierId,
        brandId,
        code,
        name,
        description,
        purchasePrice,
        currencyId,
        unitId,
        taxRateId,
        moq,
        disabled,
        remark,
        attributes,
        attachmentIds);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验属性存在性
   *
   * @param productType 产品类型
   * @param attributes 属性值（可空）
   * @throws DomainException 属性不存在时抛出
   */
  private void validateAttributes(
      String productType, @Nullable TreeMap<String, Object> attributes) {
    if (attributes != null) {
      for (String attributeCode : attributes.keySet()) {
        if (!attributeRepository.isExistByProductTypeAndScopeAndCodeExcludeId(
            productType, AttributeScopeEnum.SPU.getCode(), attributeCode)) {
          throw new DomainException("属性" + attributeCode + "不存在!");
        }
      }
    }
  }
}
