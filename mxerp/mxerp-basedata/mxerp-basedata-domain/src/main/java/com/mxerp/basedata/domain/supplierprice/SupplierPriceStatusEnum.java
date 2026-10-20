package com.mxerp.basedata.domain.supplierprice;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 供应商价格卡状态 */
@NullMarked
@Getter
public enum SupplierPriceStatusEnum implements BaseEnum {

  /** 待生效：还没到生效时间，可以继续改价 */
  PENDING("Pending", "待生效"),

  /** 当前生效：同一供应商同一产品上最多一张，取价就取它 */
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
  SupplierPriceStatusEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
