package com.mxerp.kernel.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 产品类型枚举 */
@NullMarked
@Getter
public enum ProductTypeEnum implements BaseEnum {

  /** 成品 */
  END_PRODUCT("EndProduct", "成品"),

  /** 半成品 */
  SEMI_PRODUCT("SemiProduct", "半成品"),

  /** 辅料 */
  ACCESSORY("Accessory", "辅料");

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
  ProductTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
