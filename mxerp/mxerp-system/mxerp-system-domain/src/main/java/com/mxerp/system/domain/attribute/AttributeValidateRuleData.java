package com.mxerp.system.domain.attribute;

import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * 属性校验规则数据
 *
 * @param expression 表达式
 * @param errorMessage 错误提示
 * @param sequence 排序（可空）
 */
@NullMarked
public record AttributeValidateRuleData(
    String expression, String errorMessage, @Nullable Integer sequence) {}
