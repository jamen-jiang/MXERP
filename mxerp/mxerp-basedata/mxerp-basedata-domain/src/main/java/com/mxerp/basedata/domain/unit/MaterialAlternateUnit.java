package com.mxerp.basedata.domain.unit;

import com.mxerp.shared.domain.aggregate.UpdateAggregateRoot;
import com.mxerp.shared.domain.exception.DomainException;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Index;
import jakarta.persistence.Table;
import java.math.BigDecimal;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.jspecify.annotations.NullMarked;

/**
 * 物料替代单位（只用于跨单位组）
 *
 * <p>形状照 Odoo 的 {@code product.packaging.qty}（Contained Quantity）：一个物料、一个单位、
 * 一个系数，含义是 **1 个该单位 = factor 个物料基本单位**，不存「从哪个单位到哪个单位」这一对。
 * ERPNext 的 {@code Item.uoms}（{@code uom} + {@code conversion_factor}）也是同一形状。
 *
 * <p>只收跨组的单位：跟物料基本单位同组的单位，比值由各自的 {@link Unit#getFactor()} 决定，
 * 再存一条就成了两套数、出问题不知道哪边说了算。任意两个单位之间的换算都折两次：
 * 本单位 → 物料基本单位 → 目标单位，见 {@link MaterialAlternateUnitDomainService#convert}。
 *
 * <p>系数是小数，{@code 1根 = 1/3公斤} 这种除不尽的比例只能存近似值，末位误差由目标单位的步长收。
 * 这里只存固定换算率；浮动换算（同一物料每批称出来的重量不同）在单据行上存当时的换算率，
 * 否则改一次主数据会把历史单据的量一起改掉。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "MaterialAlternateUnit",
    indexes = {
      @Index(name = "IX_MaterialAlternateUnit_MaterialId", columnList = "MaterialId")
      // 注意：(MaterialId, UnitId) 的过滤唯一索引带 WHERE IsDeleted=0，JPA 不支持，用 Flyway 创建
    })
public class MaterialAlternateUnit extends UpdateAggregateRoot {

  /** 物料Id */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** 替代单位Id（跨组的那个单位） */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 换算率：1 个替代单位 = factor 个物料基本单位 */
  @Column(name = "Factor", nullable = false, precision = 28, scale = 12)
  private BigDecimal factor;

  /**
   * 构造器
   *
   * @param materialId 物料Id
   * @param unitId 替代单位Id
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   */
  MaterialAlternateUnit(long materialId, long unitId, BigDecimal factor) {
    if (materialId <= 0 || unitId <= 0) {
      throw new DomainException("物料替代单位必须同时指定物料和单位");
    }
    this.materialId = materialId;
    this.unitId = unitId;
    apply(factor);
  }

  /**
   * 修改换算率
   *
   * <p>物料和替代单位是这一行的身份，不提供修改：要换单位就删掉重建。
   *
   * @param factor 换算率（1 个该单位 = factor 个物料基本单位）
   */
  void modify(BigDecimal factor) {
    apply(factor);
  }

  // ==================== 私有：校验与赋值 ====================

  /**
   * 统一的校验与赋值入口（构造器与修改共用）
   *
   * @param factor 换算率
   * @throws DomainException 换算率不为正数时抛出
   */
  private void apply(BigDecimal factor) {
    if (factor == null || factor.signum() <= 0) {
      throw new DomainException("物料替代单位的换算率必须大于零");
    }
    this.factor = factor;
  }
}
