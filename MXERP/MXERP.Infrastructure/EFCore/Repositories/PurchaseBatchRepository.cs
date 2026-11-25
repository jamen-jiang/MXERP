using Microsoft.EntityFrameworkCore;
using MXERP.Domain.PurchaseBatches;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 采购批次仓储
/// </summary>
[ScopedService]
public class PurchaseBatchRepository : EfCoreRepository<MXERPDbContext, PurchaseBatch>, IPurchaseBatchRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PurchaseBatchRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<PurchaseBatch>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        if (idList.Count == 0)
        {
            return new List<PurchaseBatch>();
        }

        return await (await GetQueryableAsync())
            .Include(x => x.PurchaseBatchItems)
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<PurchaseBatch>> GetListByRelationAsync(string relationType, long relationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.PurchaseBatchItems)
            .Where(x => x.RelationType == relationType && x.RelationId == relationId && x.Status != PurchaseBatchStatusEnum.Canceled)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRelationAsync(string relationType, long relationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.Status != PurchaseBatchStatusEnum.Canceled, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRelationAsync(string relationType, long relationId, long relationItemId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.RelationItemId == relationItemId && x.Status != PurchaseBatchStatusEnum.Canceled, cancellationToken);
    }
}
