package com.mxerp.kernel.enums;

import lombok.Getter;
import org.jspecify.annotations.NullMarked;

/** 业务单类型 */
@NullMarked
@Getter
public enum BusinessOrderTypeEnum implements BaseEnum {

  /** 采购单 */
  PURCHASE_ORDER("PurchaseOrder", "采购单"),

  /** 调拨单 */
  TRANSFER_ORDER("TransferOrder", "调拨单"),

  /** 镜种转换单 */
  TRANSFER_LENS_ORDER("TransferLensOrder", "镜种转换单"),

  /** 盘点单 */
  INVENTORY_ORDER("InventoryOrder", "盘点单"),

  /** 散镜订单 */
  RETAIL_ORDER("RetailOrder", "散镜订单"),

  /** 批量订单 */
  BULK_ORDER("BulkOrder", "批量订单"),

  /** 外部出库单 */
  EXTERNAL_LEAVE_STOCK_ORDER("ExternalLeaveStockOrder", "外部出库单"),

  /** 出库单 */
  LEAVE_STOCK_ORDER("LeaveStockOrder", "出库单"),

  /** 入库单 */
  ENTER_STOCK_ORDER("EnterStockOrder", "入库单"),

  /** 申购单 */
  PURCHASE_REQUISITION_ORDER("PurchaseRequisitionOrder", "申购单"),

  /** 出库批次 */
  LEAVE_STOCK_BATCH("LeaveStockBatch", "出库批次"),

  /** 采购批次 */
  PURCHASE_BATCH("PurchaseBatch", "采购批次");

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
  BusinessOrderTypeEnum(String code, String name) {
    this.code = code;
    this.name = name;
  }
}
