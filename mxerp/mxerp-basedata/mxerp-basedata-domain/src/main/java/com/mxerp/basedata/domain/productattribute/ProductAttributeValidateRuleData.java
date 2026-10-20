package com.mxerp.basedata.domain.productattribute;

import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 规格属性校验规则数据
 *
 * @param expression 表达式
 * @param errorMessage 错误提示
 * @param sort 排序（可空）
 */
@NullMarked
public record ProductAttributeValidateRuleData(
    String expression, String errorMessage, @Nullable Integer sort) {}
