using System.ComponentModel;

namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 业务单类型枚举
/// </summary>
public enum BusinessOrderTypeEnum
{
    /// <summary>
    /// 采购单
    /// </summary>
    [Description("采购单")]
    PurchaseOrder,

    /// <summary>
    /// 调拨单
    /// </summary>
    [Description("调拨单")]
    TransferOrder,

    /// <summary>
    /// 镜种转换单
    /// </summary>
    [Description("镜种转换单")]
    TransferLensOrder,

    /// <summary>
    /// 盘点单
    /// </summary>
    [Description("盘点单")]
    InventoryOrder,

    /// <summary>
    /// 散镜订单
    /// </summary>
    [Description("散镜订单")]
    RetailOrder,

    /// <summary>
    /// 批量订单
    /// </summary>
    [Description("批量订单")]
    BulkOrder,

    /// <summary>
    /// 外部出库单
    /// </summary>
    [Description("外部出库单")]
    ExternalLeaveStockOrder,

    /// <summary>
    /// 出库单
    /// </summary>
    [Description("出库单")]
    LeaveStockOrder,

    /// <summary>
    /// 入库单
    /// </summary>
    [Description("入库单")]
    EnterStockOrder,

    /// <summary>
    /// 申购单
    /// </summary>
    [Description("申购单")]
    PurchaseRequisitionOrder,

    /// <summary>
    /// 出库批次
    /// </summary>
    [Description("出库批次")]
    LeaveStockBatch,

    /// <summary>
    /// 采购批次
    /// </summary>
    [Description("采购批次")]
    PurchaseBatch
}
