using Microsoft.EntityFrameworkCore;
using MXERP.Domain.PurchasePlans;
using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 采购计划仓储
/// </summary>
[ScopedService]
public class PurchasePlanRepository : EfCoreRepository<MXERPDbContext, PurchasePlan>, IPurchasePlanRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PurchasePlanRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<PurchasePlan?> GetByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Include(x => x.PurchasePlanItems).FirstOrDefaultAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.RelationItemId == relationItemId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.RelationItemId == relationItemId, cancellationToken);
    }
}
