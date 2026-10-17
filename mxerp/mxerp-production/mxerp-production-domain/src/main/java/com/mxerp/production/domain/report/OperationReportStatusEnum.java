package com.mxerp.production.domain.report;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 工序报工单状态 */
@NullMarked
@Getter
public enum OperationReportStatusEnum implements BaseEnum {

  /** 草稿：可改可删，尚未计入工单汇总 */
  DRAFT("Draft", "草稿"),

  /** 已提交：已累加进工单工序汇总，不可再改 */
  SUBMITTED("Submitted", "已提交");

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
  OperationReportStatusEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
