package com.mxerp.shareddomain.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 眼别 */
@NullMarked
@Getter
public enum EyeTypeEnum implements BaseEnum {

  /** 双眼 */
  RL("RL", "RL"),

  /** 右眼 */
  R("R", "R"),

  /** 左眼 */
  L("L", "L");

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
  EyeTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
