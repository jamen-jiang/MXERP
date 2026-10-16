package com.mxerp.shareddomain.util;

import java.util.Map;
import java.util.TreeMap;
import java.util.stream.Collectors;

/** 关键属性工具类 */
public final class SkuAttributeUtils {

  /** 私有构造器 */
  private SkuAttributeUtils() {}

  /**
   * 关键属性拼成文本（格式：{@code key1:value1|key2:value2}）
   *
   * @param keyAttributes 关键属性值（可空）
   * @return 拼接后的文本，无属性时返回空字符串
   */
  public static String toText(TreeMap<String, Object> keyAttributes) {
    if (keyAttributes == null || keyAttributes.isEmpty()) {
      return "";
    }
    return keyAttributes.entrySet().stream()
        .sorted(Map.Entry.comparingByKey())
        .map(e -> e.getKey() + ":" + e.getValue())
        .collect(Collectors.joining("|"));
  }
}
