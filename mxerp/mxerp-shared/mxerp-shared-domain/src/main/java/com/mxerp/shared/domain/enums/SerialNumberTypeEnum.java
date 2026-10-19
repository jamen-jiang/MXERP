package com.mxerp.shared.domain.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/**
 * 流水号类型枚举
 *
 * <p>编码沿用实体表名（{@code WorkOrder} / {@code ProductionReport}），与
 * {@code OrderAddedEvent} 里带出去的类型串一致，流水号规则按这个编码挂单号前缀与编号段。一张表一个编码，表里的
 * 红字单、普通单共用一个号段。
 */
@NullMarked
@Getter
public enum SerialNumberTypeEnum implements BaseEnum {

  /** 工单（生产订单） */
  WORK_ORDER("WorkOrder", "工单"),

  /** 生产批次（合批运行，一炉/一缸/一车一张） */
  PRODUCTION_RUN("ProductionRun", "生产批次"),

  /** 生产报工单 */
  PRODUCTION_REPORT("ProductionReport", "生产报工单"),

  /** 生产领料单 */
  PRODUCTION_ISSUE("ProductionIssue", "生产领料单"),

  /** 生产退料单 */
  PRODUCTION_RETURN("ProductionReturn", "生产退料单"),

  /** 生产入库单 */
  PRODUCTION_RECEIPT("ProductionReceipt", "生产入库单"),

  /** 库存单据（一次库存动账，红字单也用这一段号） */
  STOCK_VOUCHER("StockVoucher", "库存单据");

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
  SerialNumberTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
