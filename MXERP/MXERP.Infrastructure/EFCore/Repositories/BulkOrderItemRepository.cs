using Microsoft.EntityFrameworkCore;
using MXERP.Domain.BulkOrderItems;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 批量订单项仓储
/// </summary>
[ScopedService]
public class BulkOrderItemRepository : EfCoreRepository<MXERPDbContext, BulkOrderItem>, IBulkOrderItemRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public BulkOrderItemRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<BulkOrderItem>> GetListByBulkOrderIdAsync(long bulkOrderId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.BulkOrderItemSalesSkus)
            .Where(x => x.BulkOrderId == bulkOrderId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByBulkOrderIdAndSalesSpuIdExcludeIdAsync(long bulkOrderId, long salesSpuId, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.BulkOrderId == bulkOrderId && x.SalesSpuId == salesSpuId && x.Id != id, cancellationToken);
    }
}
