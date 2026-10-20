package com.mxerp.production.domain.workcenter;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 工作中心类型 */
@NullMarked
@Getter
public enum WorkCenterTypeEnum implements BaseEnum {

  /** 设备 */
  EQUIPMENT("Equipment", "设备"),

  /** 产线 */
  LINE("Line", "产线"),

  /** 班组 */
  TEAM("Team", "班组");

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
  WorkCenterTypeEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
