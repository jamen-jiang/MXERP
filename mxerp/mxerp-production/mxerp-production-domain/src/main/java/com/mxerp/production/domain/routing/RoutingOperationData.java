package com.mxerp.production.domain.routing;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 路线工序数据
 *
 * <p>描述「这道工序在这条路线里怎么用」：顺序、工作中心、工时、取时公式与批量，以及是否外协、是否可选。
 * 这些都是路线级决策——同一个镀膜工序，A 产品自制、B 产品外协，节拍也不同，所以不能挂在工序字典上。
 * 价格不在这里，只带一个费率卡引用。
 *
 * @param sequence 工序顺序
 * @param operationId 工序Id
 * @param workCenterId 工作中心Id
 * @param setupTime 准备工时（分钟）
 * @param runTime 加工工时（分钟）
 * @param queueTime 排队工时（分钟）
 * @param rateId 费率卡Id（可空，为空表示这道工序不计加工成本）
 * @param timeFormula 工时取时公式编码
 * @param standardBatchSize 标准批量（可空，按批取时时必填）
 * @param isOutsourced 是否外协
 * @param isOptional 是否可选（决定工单上能否跳过）
 * @param reworkOperationId 返工回到的工序Id（可空）
 * @param remark 备注（可空）
 */
@NullMarked
public record RoutingOperationData(
    Integer sequence,
    Long operationId,
    Long workCenterId,
    BigDecimal setupTime,
    BigDecimal runTime,
    BigDecimal queueTime,
    @Nullable Long rateId,
    String timeFormula,
    @Nullable BigDecimal standardBatchSize,
    boolean isOutsourced,
    boolean isOptional,
    @Nullable Long reworkOperationId,
    @Nullable String remark) {}
