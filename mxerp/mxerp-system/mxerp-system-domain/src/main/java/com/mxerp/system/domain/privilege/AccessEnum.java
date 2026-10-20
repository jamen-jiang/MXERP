package com.mxerp.system.domain.privilege;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 权限通道 */
@NullMarked
@Getter
public enum AccessEnum implements BaseEnum {

  /** 应用 */
  APP("App", "应用"),

  /** 资源 */
  RESOURCE("Resource", "资源");

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
  AccessEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
