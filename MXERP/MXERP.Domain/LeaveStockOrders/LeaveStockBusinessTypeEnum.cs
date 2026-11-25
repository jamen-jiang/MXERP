using System.ComponentModel;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库业务类型
/// </summary>
public enum LeaveStockBusinessTypeEnum
{
    /// <summary>
    /// 销售出库
    /// </summary>
    [Description("销售出库")]
    SalesLeaveStock,

    /// <summary>
    /// 订单出库
    /// </summary>
    [Description("订单出库")]
    OrderLeaveStock,

    /// <summary>
    /// 盘点出库
    /// </summary>
    [Description("盘点出库")]
    InventoryLeaveStock,

    /// <summary>
    /// 转仓出库
    /// </summary>
    [Description("转仓出库")]
    TransferLeaveStock,

    /// <summary>
    /// 冲销出库
    /// </summary>
    [Description("冲销出库")]
    ReversalLeaveStock
}
