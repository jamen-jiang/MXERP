package com.mxerp.inventory.domain.statuschange;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库存状态转换单行数据
 *
 * @param materialId 物料Id（快照）
 * @param variantId 规格型号Id
 * @param warehouseId 仓库Id
 * @param locationId 库位Id（可空）
 * @param fromStockStatus 转出状态编码
 * @param toStockStatus 转入状态编码
 * @param quantity 挪动数量（正数）
 * @param remark 备注（可空）
 */
@NullMarked
public record StockStatusChangeLineData(
    Long materialId,
    Long variantId,
    Long warehouseId,
    @Nullable Long locationId,
    String fromStockStatus,
    String toStockStatus,
    BigDecimal quantity,
    @Nullable String remark) {}
