package com.mxerp.production.domain.workorder;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工单数据
 *
 * <p>只描述「要做什么、做多少、从哪来」，不含工序与用料——那两块由下达动作按 BOM 与工艺路线快照生成。
 *
 * @param skuId 产出SKU
 * @param quantity 计划数量
 * @param unitId 单位Id
 * @param sourceType 来源类型编码
 * @param sourceId 来源单据Id（可空，库存补货时为空）
 * @param sourceNumber 来源单号（可空，冗余存储便于追溯与展示）
 * @param bomId 指定BOM（可空，为空时下达自动取生效版本）
 * @param routingId 指定工艺路线（可空，为空时下达自动取生效版本）
 * @param plannedStartTime 计划开工时间（可空）
 * @param plannedEndTime 计划完工时间（可空）
 * @param wipWarehouseId 在制仓Id（可空，领料后未耗用的料存放处）
 * @param fgWarehouseId 成品仓Id（可空，完工入库目标）
 * @param scrapWarehouseId 报废仓Id（可空，报废品存放处）
 * @param remark 备注（可空）
 */
@NullMarked
public record WorkOrderData(
    Long skuId,
    BigDecimal quantity,
    Long unitId,
    String sourceType,
    @Nullable Long sourceId,
    @Nullable String sourceNumber,
    @Nullable Long bomId,
    @Nullable Long routingId,
    @Nullable LocalDateTime plannedStartTime,
    @Nullable LocalDateTime plannedEndTime,
    @Nullable Long wipWarehouseId,
    @Nullable Long fgWarehouseId,
    @Nullable Long scrapWarehouseId,
    @Nullable String remark) {}
