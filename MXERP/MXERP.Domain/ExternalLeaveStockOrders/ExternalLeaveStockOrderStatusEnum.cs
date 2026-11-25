using System.ComponentModel;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单状态(对应 Java 的 ExternalLeaveStockOrderStatusEnum, 以编码字符串存储)
/// </summary>
public enum ExternalLeaveStockOrderStatusEnum
{
    /// <summary>
    /// 待完成
    /// </summary>
    [Description("待完成")]
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
