package com.mxerp.production.domain.report;

import java.math.BigDecimal;
import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工序报工单数据
 *
 * <p>一次报工就是一条独立记录：谁、在哪道工序、什么时间段、做了多少良品多少报废。工单工序上的
 * 累计值是这些记录的汇总，而不是反过来。
 *
 * @param workOrderId 工单Id
 * @param workOrderNumber 工单号（冗余，便于展示）
 * @param workOrderOperationId 工单工序Id
 * @param operationId 工序Id（快照）
 * @param operationName 工序名称（快照）
 * @param workCenterId 工作中心Id（快照）
 * @param workCenterName 工作中心名称（快照）
 * @param operatorId 操作人Id（可空）
 * @param startTime 开始时间（可空，仅作记录）
 * @param endTime 结束时间（可空，仅作记录）
 * @param workTime 本次工时（分钟）
 * @param goodQuantity 本次良品数量
 * @param scrapQuantity 本次报废数量
 * @param remark 备注（可空）
 */
@NullMarked
public record OperationReportData(
    Long workOrderId,
    String workOrderNumber,
    Long workOrderOperationId,
    Long operationId,
    String operationName,
    Long workCenterId,
    String workCenterName,
    @Nullable Long operatorId,
    @Nullable LocalDateTime startTime,
    @Nullable LocalDateTime endTime,
    BigDecimal workTime,
    BigDecimal goodQuantity,
    BigDecimal scrapQuantity,
    @Nullable String remark) {}
