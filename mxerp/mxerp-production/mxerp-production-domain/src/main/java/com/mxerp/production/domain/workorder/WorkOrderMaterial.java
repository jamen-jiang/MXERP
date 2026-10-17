package com.mxerp.production.domain.workorder;

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
 * 工单用料
 *
 * <p>工单下达时由 BOM 展开生成：需求量 = BOM 单位用量 × 工单数量 × (1 + 损耗率)。
 *
 * <p>四个数量记录料的完整流转：要多少（required）、领多少（issued）、退多少（returned）、
 * 用多少（consumed）。差异即超领、退料或盘亏。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "WorkOrderMaterial",
    indexes = {
      @Index(name = "IX_WorkOrderMaterial_WorkOrderId", columnList = "WorkOrderId"),
      @Index(name = "IX_WorkOrderMaterial_SkuId", columnList = "SkuId")
    })
public class WorkOrderMaterial extends BaseDeleteEntity {

  /** 工单Id */
  @Column(name = "WorkOrderId")
  private Long workOrderId;

  /** 来源BOM行Id（可空，手工加料时为空） */
  @Column(name = "BomItemId")
  @Nullable
  private Long bomItemId;

  /** 物料SKU */
  @Column(name = "SkuId", nullable = false)
  private Long skuId;

  /** 需求数量 */
  @Column(name = "RequiredQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal requiredQuantity;

  /** 已领数量 */
  @Column(name = "IssuedQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal issuedQuantity;

  /** 已退数量 */
  @Column(name = "ReturnedQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal returnedQuantity;

  /** 已耗数量 */
  @Column(name = "ConsumedQuantity", nullable = false, precision = 18, scale = 4)
  private BigDecimal consumedQuantity;

  /** 单位Id */
  @Column(name = "UnitId", nullable = false)
  private Long unitId;

  /** 发料仓库Id（可空） */
  @Column(name = "WarehouseId")
  @Nullable
  private Long warehouseId;

  /** 备注（可空） */
  @Nationalized
  @Column(name = "Remark", length = 500)
  @Nullable
  private String remark;

  /**
   * 构造器
   *
   * @param bomItemId 来源BOM行Id（可空，手工加料时为空）
   * @param skuId 物料SKU
   * @param requiredQuantity 需求数量
   * @param unitId 单位Id
   * @param warehouseId 发料仓库Id（可空）
   * @param remark 备注（可空）
   */
  WorkOrderMaterial(
      @Nullable Long bomItemId,
      Long skuId,
      BigDecimal requiredQuantity,
      Long unitId,
      @Nullable Long warehouseId,
      @Nullable String remark) {
    if (skuId <= 0) {
      throw new DomainException("物料不能为空");
    }
    if (requiredQuantity.signum() < 0) {
      throw new DomainException("需求数量不能为负数");
    }
    if (unitId <= 0) {
      throw new DomainException("单位不能为空");
    }
    this.bomItemId = bomItemId;
    this.skuId = skuId;
    this.requiredQuantity = requiredQuantity;
    this.issuedQuantity = BigDecimal.ZERO;
    this.returnedQuantity = BigDecimal.ZERO;
    this.consumedQuantity = BigDecimal.ZERO;
    this.unitId = unitId;
    this.warehouseId = warehouseId;
    this.remark = remark;
  }

  /**
   * 领料：累加已领数量
   *
   * @param quantity 本次领用数量
   * @throws DomainException 数量非正时抛出
   */
  public void issue(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("领料数量必须大于0");
    }
    this.issuedQuantity = this.issuedQuantity.add(quantity);
  }

  /**
   * 退料：累加已退数量
   *
   * <p>退料只能退「已领但还没耗用」的部分，否则等于凭空多出一批料。
   *
   * @param quantity 本次退料数量
   * @throws DomainException 数量非正、或退料超过已领未耗数量时抛出
   */
  public void returnMaterial(BigDecimal quantity) {
    if (quantity.signum() <= 0) {
      throw new DomainException("退料数量必须大于0");
    }
    if (quantity.compareTo(remaining()) > 0) {
      throw new DomainException("退料数量超过已领未耗数量");
    }
    this.returnedQuantity = this.returnedQuantity.add(quantity);
  }

  /**
   * 耗用：累加已耗数量
   *
   * @param quantity 本次耗用数量
   * @throws DomainException 数量为负、或累计耗用超过已领未退数量时抛出
   */
  public void consume(BigDecimal quantity) {
    if (quantity.signum() < 0) {
      throw new DomainException("耗用数量不能为负数");
    }
    BigDecimal next = this.consumedQuantity.add(quantity);
    if (next.compareTo(this.issuedQuantity.subtract(this.returnedQuantity)) > 0) {
      throw new DomainException("累计耗用数量超过已领未退数量");
    }
    this.consumedQuantity = next;
  }

  /**
   * 已领未耗数量
   *
   * @return 已领数量减已退数量再减已耗数量
   */
  public BigDecimal remaining() {
    return this.issuedQuantity.subtract(this.returnedQuantity).subtract(this.consumedQuantity);
  }
}
