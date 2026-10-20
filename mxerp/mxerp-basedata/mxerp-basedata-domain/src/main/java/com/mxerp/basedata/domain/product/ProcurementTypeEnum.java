package com.mxerp.basedata.domain.product;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 取得方式枚举
 *
 * <p>对齐 SAP 的 {@code MARC-BESKZ}（采购类型）：E=自制、F=外购、X=两者皆可。它与「物料组/分类」是两个
 * 正交的维度，SAP 也刻意没有把它并进 {@code MARA-MATKL}——同一分类下的物料完全可以既自制又外购。
 */
@NullMarked
@Getter
public enum ProcurementTypeEnum implements BaseEnum {

  /** 自制 */
  MAKE("Make", "自制"),

  /** 外购 */
  BUY("Buy", "外购"),

  /** 自制或外购 */
  BOTH("Both", "自制或外购");

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
  ProcurementTypeEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
