package com.mxerp.system.domain.app;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 应用类型 */
@NullMarked
@Getter
public enum AppTypeEnum implements BaseEnum {

  /** 目录 */
  CATALOG("Catalog", "目录"),

  /** 应用 */
  APP("App", "应用"),

  /** 资源 */
  RESOURCE("Resource", "资源");

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
  AppTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
