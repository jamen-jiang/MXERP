package com.mxerp.basedata.domain.customermaterial;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 客户物料（客户对某个物料的叫法与编号）
 *
 * <p>纯对照表：客户 + 物料（+ 可选 SKU）+ 客户那边的代码/名称，再记一个客户订货用的单位。 价格不在这里——卖价属于价目/销售政策，等销售域落地后单独一张表。
 *
 * <p>{@code variantId} 为空表示这组码挂在物料系列上（同系列所有度数共用）；有值表示只对这个度数生效。 反查时先找变体级，再回落到系列级。
 *
 * <p>唯一性：同一客户下同一物料（同一 SKU）只能有一行，换码是改不是新增。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "CustomerMaterial",
    indexes = {
      @Index(name = "IX_CustomerMaterial_CustomerId", columnList = "CustomerId"),
      @Index(name = "IX_CustomerMaterial_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_CustomerMaterial_VariantId", columnList = "VariantId"),
      @Index(name = "IX_CustomerMaterial_CustomerProductCode", columnList = "CustomerProductCode")
      // 注意：CustomerId + MaterialId + VariantId 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，
      // 用 Flyway 创建
    })
public class CustomerMaterial extends UpdateAggregateRoot {

  /** 客户Id（不可变：换客户就是另一行） */
  @Column(name = "CustomerId", nullable = false)
  private Long customerId;

  /** 物料系列Id（不可变） */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** SKU Id（可空，不可变）：空表示这组码挂在系列上 */
  @Column(name = "VariantId")
  @Nullable
  private Long variantId;

  /** 客户产品代码（可空）：客户对这个物料的编号，下单与对账用 */
  @Nationalized
  @Column(name = "CustomerProductCode", length = 100)
  @Nullable
  private String customerProductCode;

  /** 客户产品名称（可空）：客户对这个物料的叫法 */
  @Nationalized
  @Column(name = "CustomerProductName", length = 200)
  @Nullable
  private String customerProductName;

  /**
   * 构造器
   *
   * @param customerId 客户Id
   * @param materialId 物料系列Id
   * @param variantId SKU Id（可空）
   * @param customerProductCode 客户产品代码（可空）
   * @param customerProductName 客户产品名称（可空）
   */
  CustomerMaterial(
      long customerId,
      long materialId,
      @Nullable Long variantId,
      @Nullable String customerProductCode,
      @Nullable String customerProductName) {
    this.customerId = customerId;
    this.materialId = materialId;
    this.variantId = variantId;
    apply(customerProductCode, customerProductName);
  }

  /**
   * 修改客户物料
   *
   * @param customerProductCode 客户产品代码（可空）
   * @param customerProductName 客户产品名称（可空）
   */
  void modify(@Nullable String customerProductCode, @Nullable String customerProductName) {
    apply(customerProductCode, customerProductName);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的可变字段校验与赋值入口（构造器与修改共用）
   *
   * @param customerProductCode 客户产品代码（可空）
   * @param customerProductName 客户产品名称（可空）
   */
  private void apply(@Nullable String customerProductCode, @Nullable String customerProductName) {
    this.customerProductCode = StringUtils.trimToNull(customerProductCode);
    this.customerProductName = StringUtils.trimToNull(customerProductName);
  }
}
