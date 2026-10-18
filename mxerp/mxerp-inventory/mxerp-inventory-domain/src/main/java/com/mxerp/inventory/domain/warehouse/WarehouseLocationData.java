package com.mxerp.inventory.domain.warehouse;

import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 库位数据
 *
 * @param code 库位代码（仓内唯一，同步时按它匹配已有库位）
 * @param name 库位名称
 * @param disabled 是否停用
 * @param remark 备注（可空）
 */
@NullMarked
public record WarehouseLocationData(
    String code, String name, boolean disabled, @Nullable String remark) {}
