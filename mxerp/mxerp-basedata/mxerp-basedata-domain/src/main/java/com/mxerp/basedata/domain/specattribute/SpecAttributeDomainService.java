package com.mxerp.basedata.domain.specattribute;

import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 规格属性领域服务，负责规格属性子实体同步 */
@NullMarked
public class SpecAttributeDomainService {

  /**
   * 同步规格属性值列表（按 Value 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param specAttribute 规格属性聚合
   * @param specAttributeValues 目标规格属性值列表（可空）
   */
  public void syncSpecAttributeValues(
      SpecAttribute specAttribute,
      @Nullable List<SpecAttributeValueData> specAttributeValues) {
    List<SpecAttributeValueData> target =
        specAttributeValues == null ? List.of() : specAttributeValues;

    // 1. 删除未在输入中出现的规格属性值（按 Value 匹配）
    List<SpecAttributeValue> toRemoves =
        specAttribute.getSpecAttributeValues().stream()
            .filter(i -> target.stream().noneMatch(d -> Objects.equals(d.value(), i.getValue())))
            .toList();
    for (SpecAttributeValue specAttributeValue : toRemoves) {
      specAttribute.deleteValue(specAttributeValue.getId());
    }

    // 2. 建立"Value → 已存在规格属性值Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        specAttribute.getSpecAttributeValues().stream()
            .filter(i -> !i.getIsDeleted())
            .collect(
                Collectors.toMap(
                    SpecAttributeValue::getValue,
                    SpecAttributeValue::getId,
                    (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (SpecAttributeValueData data : target) {
      Long id = existingDict.get(data.value());
      if (id != null) {
        // 存在：修改
        specAttribute.modifyValue(id, data.label(), data.isDefault(), data.sort());
      } else {
        // 不存在：添加
        specAttribute.addValue(data.value(), data.label(), data.isDefault(), data.sort());
      }
    }
  }

  /**
   * 同步规格属性校验规则列表（按 Expression 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param specAttribute 规格属性聚合
   * @param specAttributeValidateRules 目标校验规则列表（可空）
   */
  public void syncSpecAttributeValidateRules(
      SpecAttribute specAttribute,
      @Nullable List<SpecAttributeValidateRuleData> specAttributeValidateRules) {
    List<SpecAttributeValidateRuleData> target =
        specAttributeValidateRules == null
            ? List.of()
            : specAttributeValidateRules;

    // 1. 删除未在输入中出现的校验规则（按 Expression 匹配）
    List<SpecAttributeValidateRule> toRemoves =
        specAttribute.getSpecAttributeValidateRules().stream()
            .filter(
                r ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.expression(), r.getExpression())))
            .toList();
    for (SpecAttributeValidateRule rule : toRemoves) {
      specAttribute.deleteValidateRule(rule.getId());
    }

    // 2. 建立"Expression → 已存在校验规则Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        specAttribute.getSpecAttributeValidateRules().stream()
            .filter(r -> !r.getIsDeleted())
            .collect(
                Collectors.toMap(
                    SpecAttributeValidateRule::getExpression,
                    SpecAttributeValidateRule::getId,
                    (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (SpecAttributeValidateRuleData item : target) {
      Long id = existingDict.get(item.expression());
      if (id != null) {
        // 存在：修改
        specAttribute.modifyValidateRule(
            id, item.expression(), item.errorMessage(), item.sort());
      } else {
        // 不存在：添加
        specAttribute.addValidateRule(item.expression(), item.errorMessage(), item.sort());
      }
    }
  }
}
