package com.mxerp.inventory.domain.stockrecord;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库存记录行数据
 *
 * <p>数量一律为正：普通单是入或出多少，红字单是「要冲掉多少」。红字单里那个负号由单子自己落，外头填进来的永远
 * 是正数——省得谁把正负号填反了还看不出错在哪。
 *
 * @param productId 物料SPU
 * @param variantId 物料SKU
 * @param quantity 数量（正数）
 * @param warehouseId 仓库Id
 * @param locationId 库位Id（可空，空表示数量记到仓库这一层）
 * @param stockStatus 库存状态编码（见 {@code StockStatusEnum}，不动的仓就填可用）
 * @param remark 备注（可空）
 */
@NullMarked
public record StockRecordLineData(
    Long productId,
    Long variantId,
    BigDecimal quantity,
    Long warehouseId,
    @Nullable Long locationId,
    String stockStatus,
    @Nullable String remark) {}
