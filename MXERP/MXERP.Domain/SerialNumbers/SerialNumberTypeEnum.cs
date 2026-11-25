using System.ComponentModel;

namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号类型(对应 Java 的 SerialNumberTypeEnum, 以编码字符串存储)
/// </summary>
public enum SerialNumberTypeEnum
{
    /// <summary>
    /// 成品批量订单
    /// </summary>
    [Description("成品批量订单")]
    EndProductBulkOrder,

    /// <summary>
    /// 成品盘点单
    /// </summary>
    [Description("成品盘点单")]
    EndProductInventoryOrder,

    /// <summary>
    /// 成品出库批次
    /// </summary>
    [Description("成品出库批次")]
    EndProductLeaveStockBatch,

    /// <summary>
    /// 成品采购批次
    /// </summary>
    [Description("成品采购批次")]
    EndProductPurchaseBatch,

    /// <summary>
    /// 成品入库单
    /// </summary>
    [Description("成品入库单")]
    EndProductEnterStockOrder,

    /// <summary>
    /// 成品出库单
    /// </summary>
    [Description("成品出库单")]
    EndProductLeaveStockOrder,

    /// <summary>
    /// 成品采购单
    /// </summary>
    [Description("成品采购单")]
    EndProductPurchaseOrder,

    /// <summary>
    /// 半成品批量订单
    /// </summary>
    [Description("半成品批量订单")]
    SemiProductBulkOrder,

    /// <summary>
    /// 半成品盘点单
    /// </summary>
    [Description("半成品盘点单")]
    SemiProductInventoryOrder,

    /// <summary>
    /// 半成品出库批次
    /// </summary>
    [Description("半成品出库批次")]
    SemiProductLeaveStockBatch,

    /// <summary>
    /// 半成品采购批次
    /// </summary>
    [Description("半成品采购批次")]
    SemiProductPurchaseBatch,

    /// <summary>
    /// 半成品入库单
    /// </summary>
    [Description("半成品入库单")]
    SemiProductEnterStockOrder,

    /// <summary>
    /// 半成品出库单
    /// </summary>
    [Description("半成品出库单")]
    SemiProductLeaveStockOrder,

    /// <summary>
    /// 半成品采购单
    /// </summary>
    [Description("半成品采购单")]
    SemiProductPurchaseOrder,

    /// <summary>
    /// 辅料盘点单
    /// </summary>
    [Description("辅料盘点单")]
    AccessoryInventoryOrder,

    /// <summary>
    /// 辅料出库批次
    /// </summary>
    [Description("辅料出库批次")]
    AccessoryLeaveStockBatch,

    /// <summary>
    /// 辅料采购批次
    /// </summary>
    [Description("辅料采购批次")]
    AccessoryPurchaseBatch,

    /// <summary>
    /// 辅料入库单
    /// </summary>
    [Description("辅料入库单")]
    AccessoryEnterStockOrder,

    /// <summary>
    /// 辅料出库单
    /// </summary>
    [Description("辅料出库单")]
    AccessoryLeaveStockOrder,

    /// <summary>
    /// 辅料采购单
    /// </summary>
    [Description("辅料采购单")]
    AccessoryPurchaseOrder,

    /// <summary>
    /// 散镜订单
    /// </summary>
    [Description("散镜订单")]
    RetailOrder,

    /// <summary>
    /// 申购单
    /// </summary>
    [Description("申购单")]
    PurchaseRequisitionOrder,

    /// <summary>
    /// 正常出仓单
    /// </summary>
    [Description("正常出仓单")]
    NormalLeaveWarehouseOrder,

    /// <summary>
    /// 借调出仓单
    /// </summary>
    [Description("借调出仓单")]
    SecondLeaveWarehouseOrder,

    /// <summary>
    /// 发票
    /// </summary>
    [Description("发票")]
    Invoice,

    /// <summary>
    /// 三联发票
    /// </summary>
    [Description("三联发票")]
    TripleInvoice,

    /// <summary>
    /// 成品外部出库单
    /// </summary>
    [Description("成品外部出库单")]
    EndProductExternalLeaveStockOrder,

    /// <summary>
    /// 半成品外部出库单
    /// </summary>
    [Description("半成品外部出库单")]
    SemiProductExternalLeaveStockOrder
}
