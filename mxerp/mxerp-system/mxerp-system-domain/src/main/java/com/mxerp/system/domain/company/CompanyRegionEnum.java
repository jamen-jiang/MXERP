package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 公司区域 */
@NullMarked
@Getter
public enum CompanyRegionEnum implements BaseEnum {

  /** 国内 */
  DOMESTIC("Domestic", "国内"),

  /** 国际 */
  INTERNATIONAL("International", "国际");

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
  CompanyRegionEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
