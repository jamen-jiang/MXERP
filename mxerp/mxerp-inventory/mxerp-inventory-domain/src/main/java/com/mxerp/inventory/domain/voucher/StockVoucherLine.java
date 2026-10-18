package com.mxerp.inventory.domain.voucher;

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
 * 库存单据行
 *
 * <p>一行说的是「哪个 SKU、在哪个位置、多少」：仓库加库位正好对上库存表的一行，过账时就按它定位；货往哪个方向
 * 走由单据头定，整张单一个方向。SPU 只作快照，给按系列汇总和打印用。单位不抄——数量按 SKU 基本单位计这一条
 * {@code Stock} 已经定了，再抄一份单位就多一个说法。
 *
 * <p>数量带符号，红字行取负。数量只说这笔账往回走多少，正负不表示方向。负数只由红冲产生，所以「草稿里的负数行」
 * 等于「这张单是红字单」。
 */
@NullMarked
@Getter
@Setter(AccessLevel.PROTECTED)
@NoArgsConstructor(access = AccessLevel.PROTECTED)
@Entity
@Table(
    name = "StockVoucherLine",
    indexes = {
      @Index(name = "IX_StockVoucherLine_StockVoucherId", columnList = "StockVoucherId"),
      @Index(name = "IX_StockVoucherLine_SkuId", columnList = "SkuId")
    })
public class StockVoucherLine extends BaseDeleteEntity {

  /** 库存单据Id */
  @Column(name = "StockVoucherId", nullable = false)
  private Long stockVoucherId;

  /** 物料SPU（快照，仅作汇总与展示） */
  @Column(name = "SpuId", nullable = false)
  private Long spuId;

  /** 物料SKU */
  @Column(name = "SkuId", nullable = false)
  private Long skuId;

  /** 数量（SKU基本单位；红字行为负） */
  @Column(name = "Quantity", nullable = false, precision = 18, scale = 2)
  private BigDecimal quantity;

  /** 仓库Id */
  @Column(name = "WarehouseId", nullable = false)
  private Long warehouseId;

  /** 库位Id（可空，空表示数量记到仓库这一层） */
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
   * @param spuId 物料SPU（快照）
   * @param skuId 物料SKU
   * @param quantity 数量（红字行为负）
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param remark 备注（可空）
   * @throws DomainException SPU、SKU、仓库、库位非法，或数量为零时抛出
   */
  StockVoucherLine(
      Long spuId,
      Long skuId,
      BigDecimal quantity,
      Long warehouseId,
      @Nullable Long locationId,
      @Nullable String remark) {
    if (spuId <= 0) {
      throw new DomainException("物料SPU不能为空");
    }
    if (skuId <= 0) {
      throw new DomainException("物料不能为空");
    }
    if (quantity.signum() == 0) {
      throw new DomainException("数量不能为零");
    }
    if (warehouseId <= 0) {
      throw new DomainException("仓库不能为空");
    }
    if (locationId != null && locationId <= 0) {
      throw new DomainException("库位" + locationId + "有误!");
    }
    this.spuId = spuId;
    this.skuId = skuId;
    this.quantity = quantity;
    this.warehouseId = warehouseId;
    this.locationId = locationId;
    this.remark = remark;
  }
}
