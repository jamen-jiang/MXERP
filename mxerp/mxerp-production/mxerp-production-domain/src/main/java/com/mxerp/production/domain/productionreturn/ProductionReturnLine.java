package com.mxerp.production.domain.productionreturn;

import com.mxerp.shared.domain.entity.BaseDeleteEntity;
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
import org.hibernate.annotations.Nationalized;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 生产退料单行
 *
 * <p>一行说的是「哪张工单的哪条用料、退回哪个仓、多少」：工单用料行Id 是指针，SPU、SKU、单位按快照抄一份，
 * 因为单据要能独立打印与核对，不能等工单那边改版后再去看。需求量与已领量都不在这里抄——够不够退只有工单知道。
 *
 * <p>数量分两列：申请退料数量是车间报上来的，核定退料数量是仓管点收后落定的。退多了就是退料超过已领未耗， 由工单用料行自己拦。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "ProductionReturnLine",
    indexes = {
      @Index(
          name = "IX_ProductionReturnLine_ProductionReturnId",
          columnList = "ProductionReturnId"),
      @Index(
          name = "IX_ProductionReturnLine_WorkOrderMaterialId",
          columnList = "WorkOrderMaterialId"),
      @Index(name = "IX_ProductionReturnLine_MaterialId", columnList = "MaterialId"),
      @Index(name = "IX_ProductionReturnLine_VariantId", columnList = "VariantId")
    })
public class ProductionReturnLine extends BaseDeleteEntity {

  /** 生产退料单Id */
  @Column(name = "ProductionReturnId")
  private Long productionReturnId;

  /** 工单用料行Id */
  @Column(name = "WorkOrderMaterialId", nullable = false)
  private Long workOrderMaterialId;

  /** 物料SPU（快照） */
  @Column(name = "MaterialId", nullable = false)
  private Long materialId;

  /** 物料SKU（快照） */
  @Column(name = "VariantId", nullable = false)
  private Long variantId;

  /** 申请退料数量 */
  @Column(name = "AppliedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal appliedQuantity;

  /** 核定退料数量（审核时落定，0 表示尚未核定） */
  @Column(name = "ApprovedQuantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal approvedQuantity;

  /** 单位Id（快照） */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 退料仓库Id */
  @Column(name = "WarehouseId", nullable = false)
  private Long warehouseId;

  /** 库位Id（可空，空表示退回仓库这一层） */
  @Column(name = "LocationId")
  @Nullable
  private Long locationId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param workOrderMaterialId 工单用料行Id
   * @param materialId 物料SPU（快照）
   * @param variantId 物料SKU（快照）
   * @param appliedQuantity 申请退料数量
   * @param unitId 单位Id（快照）
   * @param warehouseId 退料仓库Id
   * @param locationId 库位Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException 用料行、SPU、SKU、单位、仓库非法或申请数量不为正时抛出
   */
  ProductionReturnLine(
      long workOrderMaterialId,
      long materialId,
      long variantId,
      BigDecimal appliedQuantity,
      long unitId,
      long warehouseId,
      @Nullable Long locationId,
      @Nullable String remark) {
    if (workOrderMaterialId <= 0) {
      throw new DomainException("工单用料行不能为空");
    }
    if (materialId <= 0) {
      throw new DomainException("物料SPU不能为空");
    }
    if (variantId <= 0) {
      throw new DomainException("物料不能为空");
    }
    if (appliedQuantity.signum() <= 0) {
      throw new DomainException("申请退料数量必须大于0");
    }
    if (unitId <= 0) {
      throw new DomainException("单位不能为空");
    }
    if (warehouseId <= 0) {
      throw new DomainException("退料仓库不能为空");
    }
    if (locationId != null && locationId <= 0) {
      throw new DomainException("库位" + locationId + "有误!");
    }
    this.workOrderMaterialId = workOrderMaterialId;
    this.materialId = materialId;
    this.variantId = variantId;
    this.appliedQuantity = appliedQuantity;
    this.approvedQuantity = BigDecimal.ZERO;
    this.unitId = unitId;
    this.warehouseId = warehouseId;
    this.locationId = locationId;
    this.remark = remark;
  }

  /**
   * 核定：把仓管点收的数量记上
   *
   * <p>批得比申请多没有业务依据：申请数是车间报上来的，仓管只会往下压或照收，要多退得回去改申请。
   *
   * @param quantity 核定退料数量
   * @throws DomainException 数量不为正、或大于申请数量时抛出
   */
  public void approve(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("核定退料数量必须大于0");
    }
    if (quantity.compareTo(this.appliedQuantity) > 0) {
      throw new DomainException(
          "物料" + this.variantId + "核定退料数量" + quantity + "不能大于申请数量" + this.appliedQuantity);
    }
    this.approvedQuantity = quantity;
  }

  /**
   * 是否尚未核定
   *
   * @return true 表示核定数量还是 0
   */
  public boolean isNotApproved() {
    return this.approvedQuantity.signum() == 0;
  }
}
