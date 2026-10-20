package com.mxerp.basedata.domain.productattribute;

import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 规格属性领域服务，负责规格属性子实体同步 */
@NullMarked
public class ProductAttributeDomainService {

  /**
   * 同步规格属性值列表（按 Value 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param productAttribute 规格属性聚合
   * @param productAttributeValues 目标规格属性值列表（可空）
   */
  public void syncProductAttributeValues(
      ProductAttribute productAttribute,
      @Nullable List<ProductAttributeValueData> productAttributeValues) {
    List<ProductAttributeValueData> target =
        productAttributeValues == null ? List.of() : productAttributeValues;

    // 1. 删除未在输入中出现的规格属性值（按 Value 匹配）
    List<ProductAttributeValue> toRemoves =
        productAttribute.getProductAttributeValues().stream()
            .filter(i -> target.stream().noneMatch(d -> Objects.equals(d.value(), i.getValue())))
            .toList();
    for (ProductAttributeValue productAttributeValue : toRemoves) {
      productAttribute.deleteValue(productAttributeValue.getId());
    }

    // 2. 建立"Value → 已存在规格属性值Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        productAttribute.getProductAttributeValues().stream()
            .filter(i -> !i.getIsDeleted())
            .collect(
                Collectors.toMap(
                    ProductAttributeValue::getValue, ProductAttributeValue::getId, (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (ProductAttributeValueData data : target) {
      Long id = existingDict.get(data.value());
      if (id != null) {
        // 存在：修改
        productAttribute.modifyValue(id, data.label(), data.isDefault(), data.sort());
      } else {
        // 不存在：添加
        productAttribute.addValue(data.value(), data.label(), data.isDefault(), data.sort());
      }
    }
  }

  /**
   * 同步规格属性校验规则列表（按 Expression 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param productAttribute 规格属性聚合
   * @param productAttributeValidateRules 目标校验规则列表（可空）
   */
  public void syncProductAttributeValidateRules(
      ProductAttribute productAttribute,
      @Nullable List<ProductAttributeValidateRuleData> productAttributeValidateRules) {
    List<ProductAttributeValidateRuleData> target =
        productAttributeValidateRules == null ? List.of() : productAttributeValidateRules;

    // 1. 删除未在输入中出现的校验规则（按 Expression 匹配）
    List<ProductAttributeValidateRule> toRemoves =
        productAttribute.getProductAttributeValidateRules().stream()
            .filter(
                r ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.expression(), r.getExpression())))
            .toList();
    for (ProductAttributeValidateRule rule : toRemoves) {
      productAttribute.deleteValidateRule(rule.getId());
    }

    // 2. 建立"Expression → 已存在校验规则Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        productAttribute.getProductAttributeValidateRules().stream()
            .filter(r -> !r.getIsDeleted())
            .collect(
                Collectors.toMap(
                    ProductAttributeValidateRule::getExpression,
                    ProductAttributeValidateRule::getId,
                    (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (ProductAttributeValidateRuleData item : target) {
      Long id = existingDict.get(item.expression());
      if (id != null) {
        // 存在：修改
        productAttribute.modifyValidateRule(
            id, item.expression(), item.errorMessage(), item.sort());
      } else {
        // 不存在：添加
        productAttribute.addValidateRule(item.expression(), item.errorMessage(), item.sort());
      }
    }
  }
}
