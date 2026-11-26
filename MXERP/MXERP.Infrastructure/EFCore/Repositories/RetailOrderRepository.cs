using Microsoft.EntityFrameworkCore;
using MXERP.Domain.RetailOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 散镜订单仓储
/// </summary>
[ScopedService]
public class RetailOrderRepository : EfCoreRepository<MXERPDbContext, RetailOrder>, IRetailOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public RetailOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<RetailOrder>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RetailOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.RetailOrderItems)
            .Include(x => x.RetailOrderTrackings)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RetailOrder?> GetByCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.RetailOrderItems)
            .Include(x => x.RetailOrderTrackings)
            .FirstOrDefaultAsync(x => x.CustomerOrderNumber == customerOrderNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RetailOrder?> GetWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.RetailOrderItems)
            .Include(x => x.RetailOrderTrackings)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCustomerOrderNumberAndCustomerPONumberExcludeIdAsync(string customerOrderNumber, string? customerPONumber, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.CustomerOrderNumber == customerOrderNumber && x.CustomerPONumber == customerPONumber && x.Status != nameof(RetailOrderStatusEnum.Canceled) && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCustomerPONumberExcludeIdAsync(string customerPONumber, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.CustomerPONumber == customerPONumber && x.Status != nameof(RetailOrderStatusEnum.Canceled) && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrder>> GetUnsyncedListAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => !x.IsSyncHkoTracking).Take(limit).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrderTracking>> GetUnsyncedTrackingListAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await (await GetDbContextCoreAsync()).Set<RetailOrderTracking>().Where(x => !x.IsSyncHkoTracking).Take(limit).ToListAsync(cancellationToken);
    }
}
