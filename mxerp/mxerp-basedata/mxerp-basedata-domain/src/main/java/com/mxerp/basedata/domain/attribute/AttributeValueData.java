package com.mxerp.basedata.domain.attribute;

import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 属性值数据
 *
 * @param value 属性值
 * @param name 属性值名称
 * @param isDefault 是否默认
 * @param sort 排序（可空）
 */
@NullMarked
public record AttributeValueData(
    String value, String name, Boolean isDefault, @Nullable Integer sort) {}
