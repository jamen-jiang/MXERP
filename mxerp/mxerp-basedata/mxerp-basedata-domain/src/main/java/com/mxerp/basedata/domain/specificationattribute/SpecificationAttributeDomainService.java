package com.mxerp.basedata.domain.specificationattribute;

import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 规格属性领域服务，负责规格属性子实体同步 */
@NullMarked
public class SpecificationAttributeDomainService {

  /**
   * 同步规格属性值列表（按 Value 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param specificationAttribute 规格属性聚合
   * @param specificationAttributeValues 目标规格属性值列表（可空）
   */
  public void syncSpecificationAttributeValues(
      SpecificationAttribute specificationAttribute,
      @Nullable List<SpecificationAttributeValueData> specificationAttributeValues) {
    List<SpecificationAttributeValueData> target =
        specificationAttributeValues == null ? List.of() : specificationAttributeValues;

    // 1. 删除未在输入中出现的规格属性值（按 Value 匹配）
    List<SpecificationAttributeValue> toRemoves =
        specificationAttribute.getSpecificationAttributeValues().stream()
            .filter(i -> target.stream().noneMatch(d -> Objects.equals(d.value(), i.getValue())))
            .toList();
    for (SpecificationAttributeValue specificationAttributeValue : toRemoves) {
      specificationAttribute.deleteValue(specificationAttributeValue.getId());
    }

    // 2. 建立"Value → 已存在规格属性值Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        specificationAttribute.getSpecificationAttributeValues().stream()
            .filter(i -> !i.getIsDeleted())
            .collect(
                Collectors.toMap(
                    SpecificationAttributeValue::getValue,
                    SpecificationAttributeValue::getId,
                    (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (SpecificationAttributeValueData data : target) {
      Long id = existingDict.get(data.value());
      if (id != null) {
        // 存在：修改
        specificationAttribute.modifyValue(id, data.label(), data.isDefault(), data.sort());
      } else {
        // 不存在：添加
        specificationAttribute.addValue(data.value(), data.label(), data.isDefault(), data.sort());
      }
    }
  }

  /**
   * 同步规格属性校验规则列表（按 Expression 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param specificationAttribute 规格属性聚合
   * @param specificationAttributeValidateRules 目标校验规则列表（可空）
   */
  public void syncSpecificationAttributeValidateRules(
      SpecificationAttribute specificationAttribute,
      @Nullable List<SpecificationAttributeValidateRuleData> specificationAttributeValidateRules) {
    List<SpecificationAttributeValidateRuleData> target =
        specificationAttributeValidateRules == null
            ? List.of()
            : specificationAttributeValidateRules;

    // 1. 删除未在输入中出现的校验规则（按 Expression 匹配）
    List<SpecificationAttributeValidateRule> toRemoves =
        specificationAttribute.getSpecificationAttributeValidateRules().stream()
            .filter(
                r ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.expression(), r.getExpression())))
            .toList();
    for (SpecificationAttributeValidateRule rule : toRemoves) {
      specificationAttribute.deleteValidateRule(rule.getId());
    }

    // 2. 建立"Expression → 已存在校验规则Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        specificationAttribute.getSpecificationAttributeValidateRules().stream()
            .filter(r -> !r.getIsDeleted())
            .collect(
                Collectors.toMap(
                    SpecificationAttributeValidateRule::getExpression,
                    SpecificationAttributeValidateRule::getId,
                    (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (SpecificationAttributeValidateRuleData item : target) {
      Long id = existingDict.get(item.expression());
      if (id != null) {
        // 存在：修改
        specificationAttribute.modifyValidateRule(
            id, item.expression(), item.errorMessage(), item.sort());
      } else {
        // 不存在：添加
        specificationAttribute.addValidateRule(item.expression(), item.errorMessage(), item.sort());
      }
    }
  }
}
