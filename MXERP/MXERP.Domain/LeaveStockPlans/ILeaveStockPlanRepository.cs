using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.LeaveStockPlans;

/// <summary>
/// 出库计划仓储接口
/// </summary>
public interface ILeaveStockPlanRepository : IRepository<LeaveStockPlan>
{
    /// <summary>
    /// 按关联类型 + 关联Id + 关联项Id查询（含出库计划项）, 不存在返回 null
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库计划, 不存在返回 null</returns>
    Task<LeaveStockPlan?> GetByRelationAsync(string relationType, long relationId, long? relationItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断关联类型 + 关联Id + 关联项Id是否存在
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示存在</returns>
    Task<bool> IsExistByRelationAsync(string relationType, long relationId, long? relationItemId, CancellationToken cancellationToken = default);
}
