package com.mxerp.basedata.domain.spu;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.enums.ProductTypeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.util.List;
import java.util.TreeMap;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.JdbcTypeCode;
import org.hibernate.annotations.Nationalized;
import org.hibernate.type.SqlTypes;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 产品 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Spu",
    indexes = {
      @Index(name = "IX_Spu_ProductType", columnList = "ProductType"),
      @Index(name = "IX_Spu_Group", columnList = "[Group]"),
      @Index(name = "IX_Spu_SupplierId", columnList = "SupplierId"),
      @Index(name = "IX_Spu_Code", columnList = "Code"),
      @Index(name = "IX_Spu_Name", columnList = "Name")
      // 注意：Code 的过滤唯一索引带 WHERE IsDeleted=0，用 Flyway 创建
    })
public class Spu extends UpdateAggregateRoot {

  /** 产品类型 */
  @Nationalized
  @Column(name = "ProductType", nullable = false, length = 50)
  private String productType;

  /** 供应商Id */
  @Column(name = "SupplierId", nullable = false)
  private Long supplierId;

  /** 品牌Id（可空） */
  @Column(name = "BrandId")
  @Nullable
  private Long brandId;

  /** 产品代码 */
  @Nationalized
  @Column(name = "Code", nullable = false, length = 100)
  private String code;

  /** 产品名称 */
  @Nationalized
  @Column(name = "Name", nullable = false, length = 200)
  private String name;

  /** 产品描述（可空） */
  @Nationalized
  @Column(name = "Description", length = 200)
  @Nullable
  private String description;

  /** 采购价格 */
  @Column(name = "PurchasePrice", nullable = false, precision = 18, scale = 2)
  private BigDecimal purchasePrice = BigDecimal.ZERO;

  /** 币种Id */
  @Column(name = "CurrencyId", nullable = false)
  private Long currencyId;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 税率Id（可空） */
  @Column(name = "TaxRateId")
  @Nullable
  private Long taxRateId;

  /** 起订量（可空） */
  @Column(name = "Moq", precision = 18, scale = 2)
  @Nullable
  private BigDecimal moq;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled = false;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 属性值，格式：{ "属性Code": 值, "属性Code": [值1, 值2] }
   *
   * <p>例如：{ "SPH": 25, "MinSPH": 10, "MaxSPH": 30, "Colors": ["red", "blue"] }
   */
  @JdbcTypeCode(SqlTypes.JSON)
  @Column(name = "Attributes")
  @Nullable
  private TreeMap<String, Object> attributes;

  /**
   * 构造器
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
   */
  Spu(
      String productType,
      Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      Long currencyId,
      Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      Boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    validateProductType(productType);

    // 产品类型创建后不允许修改，只在构造时赋值
    this.productType = productType;

    apply(
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
        attributes);
    addDomainEvent(new SpuSavedEvent(this, attachmentIds));
  }

  /**
   * 修改
   *
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
   */
  void modify(
      Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      Long currencyId,
      Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      Boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes,
      @Nullable List<Long> attachmentIds) {
    apply(
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
        attributes);
    addDomainEvent(new SpuSavedEvent(this, attachmentIds));
  }

  /** 删除 */
  @Override
  public void delete() {
    super.delete();
    addDomainEvent(new SpuDeletedEvent(this));
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的可变字段赋值入口（构造器与修改共用）
   *
   * <p>{@code productType} 属于不可变字段，由调用方单独校验并赋值；其余字段全部在此收敛，避免构造器与
   * {@link #modify} 两处各写一遍导致漏改。
   *
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
   */
  private void apply(
      Long supplierId,
      @Nullable Long brandId,
      String code,
      String name,
      @Nullable String description,
      BigDecimal purchasePrice,
      Long currencyId,
      Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      Boolean disabled,
      @Nullable String remark,
      @Nullable TreeMap<String, Object> attributes) {
    this.supplierId = supplierId;
    this.brandId = brandId;
    this.code = code;
    this.name = name;
    this.description = description;
    this.purchasePrice = purchasePrice;
    this.currencyId = currencyId;
    this.unitId = unitId;
    this.taxRateId = taxRateId;
    this.moq = moq;
    this.disabled = disabled;
    this.remark = remark;
    changeAttributes(attributes);
  }

  // ==================== 私有：校验 ====================

  /**
   * 校验产品类型
   *
   * @param productType 产品类型
   * @throws DomainException 类型为空或不在枚举范围内时抛出
   */
  private void validateProductType(String productType) {
    if (productType.isBlank()) {
      throw new DomainException("产品类型不能为空");
    }
    if (EnumUtils.fromCode(ProductTypeEnum.class, productType) == null) {
      throw new DomainException("产品类型" + productType + "有误!");
    }
  }

  /**
   * 变更属性值（自动包装为有序 Map）
   *
   * @param attributes 属性值（可空）
   */
  private void changeAttributes(@Nullable TreeMap<String, Object> attributes) {
    this.attributes = attributes == null ? null : new TreeMap<>(attributes);
  }
}
