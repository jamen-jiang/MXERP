using System.ComponentModel;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库业务类型
/// </summary>
public enum EnterStockBusinessTypeEnum
{
    /// <summary>
    /// 采购入库
    /// </summary>
    [Description("采购入库")]
    PurchaseEnterStock,

    /// <summary>
    /// 订单入库
    /// </summary>
    [Description("订单入库")]
    OrderEnterStock,

    /// <summary>
    /// 盘点入库
    /// </summary>
    [Description("盘点入库")]
    InventoryEnterStock,

    /// <summary>
    /// 转仓入库
    /// </summary>
    [Description("转仓入库")]
    TransferEnterStock,

    /// <summary>
    /// 冲销出库
    /// </summary>
    [Description("冲销出库")]
    ReversalEnterStock
}
