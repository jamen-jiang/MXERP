package com.mxerp.production.domain.productionreport;

import com.mxerp.shared.domain.enums.BaseEnum;
import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 生产报工单状态 */
@NullMarked
@Getter
public enum ProductionReportStatusEnum implements BaseEnum {

  /** 草稿：可改可删，尚未计入工单汇总 */
  DRAFT("Draft", "草稿"),

  /** 已提交：已累加进工单工序汇总，不可再改；要改就撤回成草稿 */
  SUBMITTED("Submitted", "已提交");

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
  ProductionReportStatusEnum(String code, String label) {
    this.code = code;
    this.label = label;
  }
}
