package com.mxerp.basedata.domain.attribute;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 数据类型 */
@NullMarked
@Getter
public enum DataTypeEnum implements BaseEnum {

  /** 字符串 */
  STRING("String", "字符串"),

  /** 整数 */
  INTEGER("Integer", "整数"),

  /** 小数 */
  DECIMAL("Decimal", "小数"),

  /** 布尔值 */
  BOOLEAN("Boolean", "布尔值"),

  /** 日期 */
  DATETIME("DateTime", "日期");

  /** 编码 */
  private final String code;

  /** 名称 */
  private final String name;

  /**
   * 构造器
   *
   * @param code 编码
   * @param name 名称
   */
  DataTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
