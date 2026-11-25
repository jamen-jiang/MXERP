using Microsoft.EntityFrameworkCore;
using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 出库计划仓储
/// </summary>
[ScopedService]
public class LeaveStockPlanRepository : EfCoreRepository<MXERPDbContext, LeaveStockPlan>, ILeaveStockPlanRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public LeaveStockPlanRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<LeaveStockPlan?> GetByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.RelationItemId == relationItemId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.RelationItemId == relationItemId, cancellationToken);
    }
}
