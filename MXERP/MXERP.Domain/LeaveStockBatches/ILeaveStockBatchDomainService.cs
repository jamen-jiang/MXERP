namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次领域服务接口
/// </summary>
public interface ILeaveStockBatchDomainService
{
    /// <summary>
    /// 添加跟踪
    /// </summary>
    /// <param name="number">出库批次号</param>
    /// <param name="workStepId">工序Id（可空）</param>
    /// <param name="reason">原因（可空）</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="inDepartmentId">当前用户关联部门Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次</returns>
    Task<LeaveStockBatch> AddTrackingAsync(string number, long? workStepId, string? reason, long employeeId, long inDepartmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按出库批次Id同步工序列表
    /// </summary>
    /// <param name="id">出库批次Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库批次</returns>
    Task<LeaveStockBatch> SyncLeaveStockBatchWorkStepsAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步出库批次工序列表
    /// </summary>
    /// <param name="leaveStockBatch">出库批次</param>
    /// <param name="workStepIds">工序Id列表（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SyncLeaveStockBatchWorkStepsAsync(LeaveStockBatch leaveStockBatch, IEnumerable<long>? workStepIds, CancellationToken cancellationToken = default);
}
