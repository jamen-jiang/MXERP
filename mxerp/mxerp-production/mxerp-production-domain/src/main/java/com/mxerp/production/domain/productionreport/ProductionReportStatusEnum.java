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

  /** 已提交：已累加进工单工序汇总，不可再改 */
  SUBMITTED("Submitted", "已提交"),

  /** 已红冲：已从工单工序汇总里减回去，原单据保留作为痕迹；报工单提交即生效，没有别的撤销去处 */
  REVERSED("Reversed", "已红冲");

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
  ProductionReportStatusEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
