using Snow.Domain.Events;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次解冻事件
/// </summary>
public class LeaveStockBatchThawStockEvent : DomainEvent
{
    /// <summary>
    /// 出库批次
    /// </summary>
    public LeaveStockBatch LeaveStockBatch { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockBatch">出库批次</param>
    public LeaveStockBatchThawStockEvent(LeaveStockBatch leaveStockBatch)
        : base(leaveStockBatch)
    {
        LeaveStockBatch = leaveStockBatch;
    }
}
