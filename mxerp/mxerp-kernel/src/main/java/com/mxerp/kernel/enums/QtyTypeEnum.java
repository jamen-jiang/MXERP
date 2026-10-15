package com.mxerp.kernel.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 数量类型 */
@NullMarked
@Getter
public enum QtyTypeEnum implements BaseEnum {

  /** 数量 */
  QTY("Qty", "数量"),

  /** 冻结库存数量 */
  FREEZE_QTY("FreezeQty", "冻结库存数量"),

  /** 上限库存数量 */
  UPPER_LIMIT_QTY("UpperLimitQty", "上限库存数量"),

  /** 下限库存数量 */
  LOWER_LIMIT_QTY("LowerLimitQty", "下限库存数量"),

  /** 已入库数量 */
  ENTER_STOCK_QTY("EnterStockQty", "已入库数量"),

  /** 已出货数量 */
  LEAVE_STOCK_QTY("LeaveStockQty", "已出货数量"),

  /** 已采购数量 */
  PURCHASE_QTY("PurchaseQty", "已采购数量"),

  /** 盘盈数量 */
  PROFIT_QTY("ProfitQty", "盘盈数量"),

  /** 盘亏数量 */
  LOSS_QTY("LossQty", "盘亏数量"),

  /** 抽仓数量 */
  DEDUCT_QTY("DeductQty", "抽仓数量"),

  /** 订购数量 */
  CUSTOMIZED_QTY("CustomizedQty", "订购数量"),

  /** 欠数 */
  OWE_QTY("OweQty", "欠数"),

  /** 实际库存 */
  ACTUAL_QTY("ActualQty", "实际库存"),

  /** 冲销数量 */
  REVERSAL_QTY("ReversalQty", "冲销数量"),

  /** 待出货数量 */
  PENDING_LEAVE_STOCK_QTY("PendingLeaveStockQty", "待出货数量"),

  /** 可出货数量 */
  CAN_LEAVE_STOCK_QTY("CanLeaveStockQty", "可出货数量");

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
  QtyTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
