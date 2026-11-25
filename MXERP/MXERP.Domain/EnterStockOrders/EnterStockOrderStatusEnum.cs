using System.ComponentModel;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单状态
/// </summary>
public enum EnterStockOrderStatusEnum
{
    /// <summary>
    /// 待入库
    /// </summary>
    [Description("待入库")]
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
