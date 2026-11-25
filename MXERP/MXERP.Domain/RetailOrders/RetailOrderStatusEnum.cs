using System.ComponentModel;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单状态(对应 Java RetailOrderStatusEnum, 以编码字符串存储)
/// </summary>
public enum RetailOrderStatusEnum
{
    /// <summary>
    /// 待审核
    /// </summary>
    [Description("待审核")]
    Pending,

    /// <summary>
    /// 待发货
    /// </summary>
    [Description("待发货")]
    Delivery,

    /// <summary>
    /// 已发货
    /// </summary>
    [Description("已发货")]
    Delivered,

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
