package com.mxerp.production.domain.productionissue;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 生产领料单行数据
 *
 * @param workOrderMaterialId 工单用料行Id
 * @param productId 物料SPU
 * @param variantId 物料SKU
 * @param appliedQuantity 申请数量
 * @param unitId 单位Id
 * @param warehouseId 发料仓库Id
 * @param locationId 库位Id（可空，空表示从仓库这一层发）
 * @param remark 备注（可空）
 */
@NullMarked
public record ProductionIssueLineData(
    Long workOrderMaterialId,
    Long productId,
    Long variantId,
    BigDecimal appliedQuantity,
    Long unitId,
    Long warehouseId,
    @Nullable Long locationId,
    @Nullable String remark) {}
