using System.ComponentModel;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次状态
/// </summary>
public enum PurchaseBatchStatusEnum
{
    /// <summary>
    /// 待采购
    /// </summary>
    [Description("待采购")]
    Pending,

    /// <summary>
    /// 部分采购
    /// </summary>
    [Description("部分采购")]
    PartialPurchased,

    /// <summary>
    /// 全部采购
    /// </summary>
    [Description("全部采购")]
    AllPurchased,

    /// <summary>
    /// 超出采购
    /// </summary>
    [Description("超出采购")]
    OverPurchased,

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
