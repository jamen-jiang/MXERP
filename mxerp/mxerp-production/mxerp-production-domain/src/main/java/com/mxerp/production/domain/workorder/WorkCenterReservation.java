package com.mxerp.production.domain.workorder;

import java.time.LocalDateTime;
import org.jspecify.annotations.NullMarked;

/**
 * 工作中心上已被占用的时段
 *
 * <p>只用于排产读取：别的工单已经把某台设备排在哪个时段，排新单时要绕开。它不是实体，也不落库——
 * 占用时段本来就是工序上的计划起止时间，查出来拼成这个形状方便算。
 *
 * @param workOrderId 占用它的工单Id（批量排产时用来剔除自己上一次的结果）
 * @param workCenterId 工作中心Id
 * @param startTime 占用开始
 * @param endTime 占用结束
 */
@NullMarked
public record WorkCenterReservation(
    Long workOrderId, Long workCenterId, LocalDateTime startTime, LocalDateTime endTime) {}
