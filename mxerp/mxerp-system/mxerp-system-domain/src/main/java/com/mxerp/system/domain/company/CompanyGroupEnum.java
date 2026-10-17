package com.mxerp.system.domain.company;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 公司组别 */
@NullMarked
@Getter
public enum CompanyGroupEnum implements BaseEnum {

  /** 海外 */
  OVERSEAS("Overseas", "海外"),

  /** 东南亚 */
  SEA("Sea", "东南亚"),

  /** 其他分工厂 */
  OTHER("Other", "其他分工厂"),

  /** 国内 */
  DOMESTIC("Domestic", "国内");

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
  CompanyGroupEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
