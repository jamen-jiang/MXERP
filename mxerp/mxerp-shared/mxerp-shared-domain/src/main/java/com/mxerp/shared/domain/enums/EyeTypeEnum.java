package com.mxerp.shared.domain.enums;

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

  /** 标签 */
  private final String label;

  /**
   * 构造器
   *
   * @param code 编码
   * @param label 标签
   */
  EyeTypeEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
