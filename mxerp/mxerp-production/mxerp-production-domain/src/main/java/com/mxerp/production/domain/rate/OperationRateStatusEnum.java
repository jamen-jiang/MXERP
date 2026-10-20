package com.mxerp.production.domain.rate;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 工序费率卡状态 */
@NullMarked
@Getter
public enum OperationRateStatusEnum implements BaseEnum {

  /** 待生效：还没到生效时间，可以继续改价 */
  PENDING("Pending", "待生效"),

  /** 当前生效：同一代码下最多一张，取价就取它 */
  CURRENT("Current", "当前生效"),

  /** 历史：已被新卡接替，只用于追溯，不能再取价 */
  PREVIOUS("Previous", "历史");

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
  OperationRateStatusEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
