using System.ComponentModel;

namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单状态(对应 Java 的 BulkOrderStatusEnum, 以编码字符串存储)
/// </summary>
public enum BulkOrderStatusEnum
{
    /// <summary>
    /// 待发货
    /// </summary>
    [Description("待发货")]
    Pending,

    /// <summary>
    /// 部分发货
    /// </summary>
    [Description("部分发货")]
    PartialDelivered,

    /// <summary>
    /// 全部发货
    /// </summary>
    [Description("全部发货")]
    AllDelivered,

    /// <summary>
    /// 超出发货
    /// </summary>
    [Description("超出发货")]
    OverDelivered,

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
