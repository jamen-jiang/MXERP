package com.mxerp.basedata.domain.productattribute;

import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 规格属性值数据
 *
 * @param value 规格属性值
 * @param label 规格属性值标签
 * @param isDefault 是否默认
 * @param sort 排序（可空）
 */
@NullMarked
public record ProductAttributeValueData(
    String value, String label, Boolean isDefault, @Nullable Integer sort) {}
