package com.mxerp.inventory.domain.stock;

import com.mxerp.shared.domain.repository.Repository;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 库存仓储接口 */
@NullMarked
public interface StockRepository extends Repository<Stock> {

  /**
   * 按「SKU + 仓库 + 库位」取库存行
   *
   * <p>库位为空时只匹配同样没有库位的行：SQL 里要用 {@code LocationId IS NULL} 而不是 {@code = NULL}，
   * 否则永远查不到未分拣到货架的那一行。
   *
   * @param skuId SKU
   * @param warehouseId 仓库Id
   * @param locationId 库位Id（可空）
   * @return 库存行（可空），该键还没有记录时返回 null
   */
  @Nullable Stock getByKey(Long skuId, Long warehouseId, @Nullable Long locationId);
}
