using Snow.Domain.Events;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次完成事件
/// </summary>
public class LeaveStockBatchFinishedEvent : DomainEvent
{
    /// <summary>
    /// 出库批次
    /// </summary>
    public LeaveStockBatch LeaveStockBatch { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockBatch">出库批次</param>
    public LeaveStockBatchFinishedEvent(LeaveStockBatch leaveStockBatch)
        : base(leaveStockBatch)
    {
        LeaveStockBatch = leaveStockBatch;
    }
}
