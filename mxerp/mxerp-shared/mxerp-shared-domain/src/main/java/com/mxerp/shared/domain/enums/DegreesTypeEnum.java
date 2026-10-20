package com.mxerp.shared.domain.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 度数类型 */
@NullMarked
@Getter
public enum DegreesTypeEnum implements BaseEnum {

  /** 正散（SPH > 0） */
  ZHENG_SAN("+/-", "正散"),

  /** 负散（SPH <= 0） */
  FU_SAN("-/-", "负散");

  /** 编码 */
  private final String code;

  /** 标签 */
  private final String label;

  DegreesTypeEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
