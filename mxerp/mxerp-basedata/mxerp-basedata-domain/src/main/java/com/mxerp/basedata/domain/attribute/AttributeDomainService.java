package com.mxerp.basedata.domain.attribute;

import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.stream.Collectors;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/** 属性领域服务，负责属性子实体同步 */
@NullMarked
public class AttributeDomainService {

  /**
   * 同步属性项列表（按 Value 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param attribute 属性聚合
   * @param attributeItems 目标属性项列表（可空）
   */
  public void syncAttributeItems(
      Attribute attribute, @Nullable List<AttributeItemData> attributeItems) {
    List<AttributeItemData> target = attributeItems == null ? List.of() : attributeItems;

    // 1. 删除未在输入中出现的属性项（按 Value 匹配）
    List<AttributeItem> toRemoves =
        attribute.getAttributeItems().stream()
            .filter(i -> target.stream().noneMatch(d -> Objects.equals(d.value(), i.getValue())))
            .toList();
    for (AttributeItem item : toRemoves) {
      attribute.deleteItem(item.getId());
    }

    // 2. 建立"Value → 已存在属性项Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        attribute.getAttributeItems().stream()
            .filter(i -> !i.getIsDeleted())
            .collect(Collectors.toMap(AttributeItem::getValue, AttributeItem::getId, (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (AttributeItemData item : target) {
      Long id = existingDict.get(item.value());
      if (id != null) {
        // 存在：修改
        attribute.modifyItem(id, item.name(), item.isDefault(), item.sort());
      } else {
        // 不存在：添加
        attribute.addItem(item.value(), item.name(), item.isDefault(), item.sort());
      }
    }
  }

  /**
   * 同步属性校验规则列表（按 Expression 业务标识匹配：删除不在输入中的、更新已存在的、添加新的）
   *
   * @param attribute 属性聚合
   * @param attributeValidateRules 目标校验规则列表（可空）
   */
  public void syncAttributeValidateRules(
      Attribute attribute, @Nullable List<AttributeValidateRuleData> attributeValidateRules) {
    List<AttributeValidateRuleData> target =
        attributeValidateRules == null ? List.of() : attributeValidateRules;

    // 1. 删除未在输入中出现的校验规则（按 Expression 匹配）
    List<AttributeValidateRule> toRemoves =
        attribute.getAttributeValidateRules().stream()
            .filter(
                r ->
                    target.stream()
                        .noneMatch(d -> Objects.equals(d.expression(), r.getExpression())))
            .toList();
    for (AttributeValidateRule rule : toRemoves) {
      attribute.deleteValidateRule(rule.getId());
    }

    // 2. 建立"Expression → 已存在校验规则Id"的映射（只统计未删除的）
    Map<String, Long> existingDict =
        attribute.getAttributeValidateRules().stream()
            .filter(r -> !r.getIsDeleted())
            .collect(
                Collectors.toMap(
                    AttributeValidateRule::getExpression,
                    AttributeValidateRule::getId,
                    (a, b) -> a));

    // 3. 遍历输入：存在则修改，不存在则添加
    for (AttributeValidateRuleData item : target) {
      Long id = existingDict.get(item.expression());
      if (id != null) {
        // 存在：修改
        attribute.modifyValidateRule(id, item.expression(), item.errorMessage(), item.sort());
      } else {
        // 不存在：添加
        attribute.addValidateRule(item.expression(), item.errorMessage(), item.sort());
      }
    }
  }
}
