using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次工序
/// </summary>
public class LeaveStockBatchWorkStep : DeletionAuditedEntity
{
    /// <summary>
    /// 出库批次Id
    /// </summary>
    public long? LeaveStockBatchId { get; private set; }

    /// <summary>
    /// 工序Id
    /// </summary>
    public long WorkStepId { get; private set; }

    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveStockBatchWorkStep()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    internal LeaveStockBatchWorkStep(long workStepId)
    {
        WorkStepId = workStepId;
    }
}
