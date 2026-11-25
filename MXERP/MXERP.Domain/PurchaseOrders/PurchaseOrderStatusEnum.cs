using System.ComponentModel;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单状态
/// </summary>
public enum PurchaseOrderStatusEnum
{
    /// <summary>
    /// 待审核
    /// </summary>
    [Description("待审核")]
    Pending,

    /// <summary>
    /// 待入库
    /// </summary>
    [Description("待入库")]
    EnterStock,

    /// <summary>
    /// 部分入库
    /// </summary>
    [Description("部分入库")]
    PartialEnterStock,

    /// <summary>
    /// 全部入库
    /// </summary>
    [Description("全部入库")]
    AllEnterStock,

    /// <summary>
    /// 超出入库
    /// </summary>
    [Description("超出入库")]
    OverEnterStock,

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
