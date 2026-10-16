package com.mxerp.shareddomain.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 日期类型 */
@NullMarked
@Getter
public enum DateTypeEnum implements BaseEnum {

  /** 年 */
  YEAR("Year", "年"),

  /** 月 */
  MONTH("Month", "月"),

  /** 日 */
  DAY("Day", "日");

  /** 编码 */
  private final String code;

  /** 名称 */
  private final String name;

  DateTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
