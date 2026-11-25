using Microsoft.EntityFrameworkCore;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 出库批次仓储
/// </summary>
[ScopedService]
public class LeaveStockBatchRepository : EfCoreRepository<MXERPDbContext, LeaveStockBatch>, ILeaveStockBatchRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public LeaveStockBatchRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<LeaveStockBatch>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        return await (await GetQueryableAsync()).Where(x => idList.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LeaveStockBatch?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Number == number && x.Status != LeaveStockBatchStatusEnum.Canceled, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<LeaveStockBatch>> GetListByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.RelationType == relationType && x.RelationId == relationId && x.Status != LeaveStockBatchStatusEnum.Canceled).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<LeaveStockBatch>> GetListByPurchaseBatchIdAsync(long purchaseBatchId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.PurchaseBatchId == purchaseBatchId && x.Status != LeaveStockBatchStatusEnum.Canceled).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.Status != LeaveStockBatchStatusEnum.Canceled, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByRelationAsync(BusinessOrderTypeEnum relationType, long relationId, long relationItemId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.RelationType == relationType && x.RelationId == relationId && x.RelationItemId == relationItemId && x.Status != LeaveStockBatchStatusEnum.Canceled, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByPurchaseBatchIdAsync(long purchaseBatchId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.PurchaseBatchId == purchaseBatchId && x.Status != LeaveStockBatchStatusEnum.Canceled, cancellationToken);
    }
}
