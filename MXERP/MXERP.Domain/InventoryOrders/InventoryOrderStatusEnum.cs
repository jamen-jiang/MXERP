using System.ComponentModel;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单订单状态
/// </summary>
public enum InventoryOrderStatusEnum
{
    /// <summary>
    /// 待审核
    /// </summary>
    [Description("待审核")]
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
