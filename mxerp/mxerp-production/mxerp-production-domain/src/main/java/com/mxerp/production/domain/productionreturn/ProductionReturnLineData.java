package com.mxerp.production.domain.productionreturn;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 生产退料单行数据
 *
 * @param workOrderMaterialId 工单用料行Id
 * @param materialId 物料SPU
 * @param variantId 物料SKU
 * @param appliedQty 申请退料数量
 * @param unitId 单位Id
 * @param warehouseId 退料仓库Id
 * @param locationId 库位Id（可空，空表示退回仓库这一层）
 * @param remark 备注（可空）
 */
@NullMarked
public record ProductionReturnLineData(
    Long workOrderMaterialId,
    Long materialId,
    Long variantId,
    BigDecimal appliedQty,
    Long unitId,
    Long warehouseId,
    @Nullable Long locationId,
    @Nullable String remark) {}
