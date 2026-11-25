using System.ComponentModel;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单状态
/// </summary>
public enum LeaveStockOrderStatusEnum
{
    /// <summary>
    /// 待出库
    /// </summary>
    [Description("待出库")]
    Pending,

    /// <summary>
    /// 已完成
    /// </summary>
    [Description("已完成")]
    Finished,

    /// <summary>
    /// 已取消
    /// </summary>
    [Description("已取消")]
    Canceled
}
