package com.mxerp.shareddomain.contract;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 唯一 SKU 信息数据 */
@NullMarked
@Getter
public class UniqueSkuInfoData {

  /** 唯一标识 */
  private final Long id;

  /** 产品SkuId */
  private final Long skuId;

  /** 仓库Id */
  private final Long warehouseId;

  /** 数量 */
  private final Double qty;

  /**
   * 构造器（qty 可空，null 时视为 0）
   *
   * @param id 唯一标识
   * @param skuId 产品SkuId
   * @param warehouseId 仓库Id
   * @param qty 数量（可空）
   */
  public UniqueSkuInfoData(Long id, Long skuId, Long warehouseId, @Nullable Double qty) {
    this.id = id;
    this.skuId = skuId;
    this.warehouseId = warehouseId;
    this.qty = qty == null ? 0.0 : qty;
  }
}
