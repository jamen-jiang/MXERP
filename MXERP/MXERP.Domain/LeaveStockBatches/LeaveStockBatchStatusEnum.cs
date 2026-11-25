using System.ComponentModel;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次状态
/// </summary>
public enum LeaveStockBatchStatusEnum
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
    /// 已开单
    /// </summary>
    [Description("已开单")]
    BillIssued,

    /// <summary>
    /// 已取消
    /// </summary>
    [Description("已取消")]
    Canceled
}
