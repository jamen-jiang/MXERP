namespace MXERP.Domain.LeaveStockPlans;

/// <summary>
/// 出库计划领域服务接口
/// </summary>
public interface ILeaveStockPlanDomainService
{
    /// <summary>
    /// 同步出库计划项列表
    /// </summary>
    /// <param name="leaveStockPlan">出库计划</param>
    /// <param name="skuInfos">出库计划项数据列表（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SyncLeaveStockPlanItemsAsync(LeaveStockPlan leaveStockPlan, IReadOnlyCollection<LeaveStockPlanItemData>? skuInfos, CancellationToken cancellationToken = default);
}
