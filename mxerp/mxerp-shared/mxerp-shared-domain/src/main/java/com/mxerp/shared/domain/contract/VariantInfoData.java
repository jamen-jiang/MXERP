package com.mxerp.shared.domain.contract;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * SKU 信息数据
 *
 * <p>用于承载 SKU、仓库及数量信息。
 */
@NullMarked
@Getter
public class VariantInfoData {

  /** 物料VariantId */
  private final Long variantId;

  /** 仓库Id */
  private final Long warehouseId;

  /** 数量 */
  private final Double qty;

  /**
   * 构造器（qty 可空，null 时视为 0）
   *
   * @param variantId 物料VariantId
   * @param warehouseId 仓库Id
   * @param qty 数量（可空）
   */
  public VariantInfoData(Long variantId, Long warehouseId, @Nullable Double qty) {
    this.variantId = variantId;
    this.warehouseId = warehouseId;
    this.qty = qty == null ? 0.0 : qty;
  }
}
