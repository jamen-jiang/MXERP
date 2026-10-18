package com.mxerp.production.domain.routing;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 工艺路线状态 */
@NullMarked
@Getter
public enum RoutingStatusEnum implements BaseEnum {

  /** 草稿：可随意改工序 */
  DRAFT("Draft", "草稿"),

  /** 已发布：可用于工单下达，改动需升版 */
  RELEASED("Released", "已发布"),

  /** 已作废：新工单不可用，历史工单不受影响 */
  OBSOLETE("Obsolete", "已作废");

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
  RoutingStatusEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
