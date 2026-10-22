package com.mxerp.inventory.domain.stock;

import com.mxerp.shared.domain.repository.Repository;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 库存仓储接口 */
@NullMarked
public interface StockRepository extends Repository<Stock> {

  /**
   * 按「SKU + 仓库 + 库位 + 库存状态」取库存行
   *
   * <p>库位为空时只匹配同样没有库位的行：SQL 里要用 {@code LocationId IS NULL} 而不是 {@code = NULL}，
   * 否则永远查不到未分拣到货架的那一行。状态不可空，同一个位置上的待检与可用是两行。
   *
   * @param variantId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @param stockStatus 库存状态编码
   * @return 库存行（可空），该键还没有记录时返回 null
   */
  @Nullable Stock getByKey(
      long variantId, long warehouseId, @Nullable Long locationId, String stockStatus);
}
