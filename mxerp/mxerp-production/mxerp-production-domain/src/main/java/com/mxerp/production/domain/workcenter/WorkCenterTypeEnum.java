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

  /** 名称 */
  private final String name;

  /**
   * 构造器
   *
   * @param code 编码
   * @param name 名称
   */
  WorkCenterTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
