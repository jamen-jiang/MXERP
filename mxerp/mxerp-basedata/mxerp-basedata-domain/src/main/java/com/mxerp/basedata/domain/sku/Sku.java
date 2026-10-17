package com.mxerp.basedata.domain.sku;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.enums.ProductTypeEnum;
import com.mxerp.shared.domain.exception.DomainException;
import com.mxerp.shared.domain.util.EnumUtils;
import com.mxerp.shared.domain.util.HashUtils;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import java.util.TreeMap;
import java.util.stream.Collectors;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.JdbcTypeCode;
import org.hibernate.annotations.Nationalized;
import org.hibernate.type.SqlTypes;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** SKU */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "Sku",
    indexes = {
      @Index(name = "IX_Sku_ProductType", columnList = "ProductType"),
      @Index(name = "IX_Sku_SpuId", columnList = "SpuId"),
      @Index(name = "IX_Sku_Opc", columnList = "Opc")
      // 注意：AttributesSignature 的过滤唯一索引带 WHERE IsDeleted=0，用 Flyway 创建
      // 注意：SpuId + Opc 的过滤唯一索引带 WHERE IsDeleted=0 AND Opc IS NOT NULL AND Opc<>''，用 Flyway 创建
    })
public class Sku extends UpdateAggregateRoot {

  /** 产品类型 */
  @Nationalized
  @Column(name = "ProductType", nullable = false, length = 50)
  private String productType;

  /** SpuId */
  @Column(name = "SpuId", nullable = false)
  private Long spuId;

  /** 属性标识码 */
  @Nationalized
  @Column(name = "AttributesSignature", nullable = false, length = 100)
  private String attributesSignature;

  /** OPC（可空） */
  @Nationalized
  @Column(name = "Opc", length = 100)
  @Nullable
  private String opc;

  /** 采购价格（可空） */
  @Column(name = "PurchasePrice", precision = 18, scale = 2)
  @Nullable
  private BigDecimal purchasePrice;

  /** 币种Id（可空） */
  @Column(name = "CurrencyId")
  @Nullable
  private Long currencyId;

  /** 单位Id（可空） */
  @Column(name = "UnitId")
  @Nullable
  private Long unitId;

  /** 税率Id（可空） */
  @Column(name = "TaxRateId")
  @Nullable
  private Long taxRateId;

  /** 是否禁用 */
  @Column(name = "Disabled", nullable = false)
  private Boolean disabled = false;

  /** 起订量（可空） */
  @Column(name = "Moq", precision = 18, scale = 2)
  @Nullable
  private BigDecimal moq;

  /** 关键属性值（参与唯一性判断），格式：{ "SPH": 25, "CYL": -25 } */
  @JdbcTypeCode(SqlTypes.JSON)
  @Column(name = "KeyAttributes")
  @Nullable
  private TreeMap<String, Object> keyAttributes;

  /** 非关键属性值（不参与唯一性判断），格式：{ "Weight": 0.5, "Material": "棉" } */
  @JdbcTypeCode(SqlTypes.JSON)
  @Column(name = "OtherAttributes")
  @Nullable
  private TreeMap<String, Object> otherAttributes;

  /**
   * 构造器
   *
   * @param productType 产品类型
   * @param spuId SpuId
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param keyAttributes 关键属性值（可空）
   * @param otherAttributes 非关键属性值（可空）
   */
  Sku(
      String productType,
      Long spuId,
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> keyAttributes,
      @Nullable TreeMap<String, Object> otherAttributes) {
    validateProductType(productType);

    // 不可变字段：产品类型、SpuId、关键属性值（决定唯一性签名）创建后不允许修改
    this.productType = productType;
    this.spuId = spuId;
    changeKeyAttributes(keyAttributes);

    // 可变字段交给 apply 统一处理
    apply(opc, purchasePrice, currencyId, unitId, taxRateId, moq, otherAttributes);

    // 签名依赖 spuId + keyAttributes，必须在两者赋值完成后计算
    this.attributesSignature = calculateSignature();
  }

  /**
   * 修改
   *
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param otherAttributes 非关键属性值（可空）
   */
  void modify(
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> otherAttributes) {
    apply(opc, purchasePrice, currencyId, unitId, taxRateId, moq, otherAttributes);
  }

  /** 启用 */
  public void enable() {
    this.disabled = false;
  }

  /** 禁用 */
  public void disable() {
    this.disabled = true;
  }

  /** 删除 */
  @Override
  public void delete() {
    super.delete();
    addDomainEvent(new SkuDeletedEvent(this));
  }

  // ---------- 计算属性 ----------

  /**
   * 是否有关键属性
   *
   * @return true：有关键属性
   */
  public boolean hasKeyAttributes() {
    Long id = getId();
    return !attributesSignature.isBlank() && id > 0;
  }

  /**
   * 合并所有属性（用于验证时方便）
   *
   * @return 合并后的属性
   */
  public TreeMap<String, Object> attributes() {
    TreeMap<String, Object> all = new TreeMap<>();
    if (keyAttributes != null) {
      all.putAll(keyAttributes);
    }
    if (otherAttributes != null) {
      all.putAll(otherAttributes);
    }
    return all;
  }

  // ---------- 签名计算 ----------

  /**
   * 静态工具方法：在不创建实例的情况下计算特征值。 适用于批量验证或查询场景。
   *
   * @param spuId SpuId
   * @param keyAttributes 关键属性值（可空）
   * @return MD5 特征值
   */
  public static String calculateSignature(
      Long spuId, @Nullable TreeMap<String, Object> keyAttributes) {
    StringBuilder builder = new StringBuilder(String.valueOf(spuId));
    if (keyAttributes != null && !keyAttributes.isEmpty()) {
      TreeMap<String, Object> ordered = new TreeMap<>(keyAttributes);
      String joined =
          ordered.values().stream()
              .map(v -> v == null ? "null" : v.toString())
              .collect(Collectors.joining("|"));
      builder.append('|').append(joined);
    }
    return HashUtils.computeMd5(builder.toString());
  }

  /**
   * 私有方法：计算特征值
   *
   * @return MD5 特征值
   */
  private String calculateSignature() {
    StringBuilder builder = new StringBuilder(String.valueOf(spuId));
    if (keyAttributes != null && !keyAttributes.isEmpty()) {
      TreeMap<String, Object> ordered = new TreeMap<>(keyAttributes);
      String joined =
          ordered.values().stream()
              .map(v -> v == null ? "null" : v.toString())
              .collect(Collectors.joining("|"));
      builder.append('|').append(joined);
    }
    return HashUtils.computeMd5(builder.toString());
  }

  // ---------- 校验 ----------

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

  // ---------- 变更字段 ----------

  /**
   * 统一的可变字段赋值入口（构造器与修改共用）
   *
   * <p>{@code productType}、{@code spuId}、{@code keyAttributes} 属于不可变字段（关键属性决定唯一性签名），
   * 由调用方单独赋值；其余字段全部在此收敛，避免构造器与 {@link #modify} 两处各写一遍导致漏改。
   *
   * @param opc OPC（可空）
   * @param purchasePrice 采购价格（可空）
   * @param currencyId 币种Id（可空）
   * @param unitId 单位Id（可空）
   * @param taxRateId 税率Id（可空）
   * @param moq 起订量（可空）
   * @param otherAttributes 非关键属性值（可空）
   */
  private void apply(
      @Nullable String opc,
      @Nullable BigDecimal purchasePrice,
      @Nullable Long currencyId,
      @Nullable Long unitId,
      @Nullable Long taxRateId,
      @Nullable BigDecimal moq,
      @Nullable TreeMap<String, Object> otherAttributes) {
    this.opc = opc;
    this.purchasePrice = purchasePrice;
    this.currencyId = currencyId;
    this.unitId = unitId;
    this.taxRateId = taxRateId;
    this.moq = moq;
    changeOtherAttributes(otherAttributes);
  }

  /**
   * 变更关键属性值（自动包装为有序 Map）
   *
   * @param attributes 关键属性值（可空）
   */
  private void changeKeyAttributes(@Nullable TreeMap<String, Object> attributes) {
    this.keyAttributes = attributes == null ? null : new TreeMap<>(attributes);
  }

  /**
   * 变更非关键属性值（自动包装为有序 Map）
   *
   * @param attributes 非关键属性值（可空）
   */
  private void changeOtherAttributes(@Nullable TreeMap<String, Object> attributes) {
    this.otherAttributes = attributes == null ? null : new TreeMap<>(attributes);
  }
}
