package com.mxerp.production.domain.operation;

import java.math.BigDecimal;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 工序数据
 *
 * <p>只承载工序字典本身的 8 个字段；「是否外协」「是否可选」这类使用决策在
 * {@link com.mxerp.production.domain.routing.RoutingOperationData} 里。
 *
 * @param code 工序代码
 * @param name 工序名称
 * @param defaultWorkCenterId 默认工作中心（可空，建路线时预填）
 * @param batchSize 标准批量（可空，如镀膜一炉 20 片）
 * @param isCorrective 是否返修工序
 * @param qualityInspectionRequired 是否需要质检
 * @param description 工序说明（可空）
 * @param disabled 是否禁用
 */
@NullMarked
public record OperationData(
    String code,
    String name,
    @Nullable Long defaultWorkCenterId,
    @Nullable BigDecimal batchSize,
    boolean isCorrective,
    boolean qualityInspectionRequired,
    @Nullable String description,
    boolean disabled) {}
