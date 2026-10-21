package com.mxerp.shared.domain.contract;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 唯一 SKU 信息数据 */
@NullMarked
@Getter
public class UniqueVariantInfoData {

  /** 唯一标识 */
  private final Long id;

  /** 物料VariantId */
  private final Long variantId;

  /** 仓库Id */
  private final Long warehouseId;

  /** 数量 */
  private final double qty;

  /**
   * 构造器（qty 可空，null 时视为 0）
   *
   * @param id 唯一标识
   * @param variantId 物料VariantId
   * @param warehouseId 仓库Id
   * @param qty 数量（可空）
   */
  public UniqueVariantInfoData(long id, long variantId, long warehouseId, @Nullable Double qty) {
    this.id = id;
    this.variantId = variantId;
    this.warehouseId = warehouseId;
    this.qty = qty == null ? 0.0 : qty;
  }
}
