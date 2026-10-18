package com.mxerp.inventory.domain.voucher;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库存单据行数据
 *
 * <p>数量一律为正：普通单是入或出多少，红字单是「要冲掉多少」。红字单里那个负号由单子自己落，外头填进来的永远
 * 是正数——省得谁把正负号填反了还看不出错在哪。
 *
 * @param spuId 物料SPU
 * @param skuId 物料SKU
 * @param quantity 数量（正数）
 * @param warehouseId 仓库Id
 * @param locationId 库位Id（可空，空表示数量记到仓库这一层）
 * @param remark 备注（可空）
 */
@NullMarked
public record StockVoucherLineData(
    Long spuId,
    Long skuId,
    BigDecimal quantity,
    Long warehouseId,
    @Nullable Long locationId,
    @Nullable String remark) {}
