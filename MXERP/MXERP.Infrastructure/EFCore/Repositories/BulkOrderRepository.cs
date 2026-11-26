using Microsoft.EntityFrameworkCore;
using MXERP.Domain.BulkOrders;
using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 批量订单仓储
/// </summary>
[ScopedService]
public class BulkOrderRepository : EfCoreRepository<MXERPDbContext, BulkOrder>, IBulkOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public BulkOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<BulkOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCustomerOrderNumberAndCustomerPONumberAndProductTypeExcludeIdAsync(string customerOrderNumber, string? customerPONumber, string productType, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.CustomerOrderNumber == customerOrderNumber && x.CustomerPONumber == customerPONumber && x.ProductType == productType && x.Id != id && x.Status != nameof(BulkOrderStatusEnum.Canceled), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCustomerPONumberAndProductTypeExcludeIdAsync(string customerPONumber, string productType, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.CustomerPONumber == customerPONumber && x.ProductType == productType && x.Id != id && x.Status != nameof(BulkOrderStatusEnum.Canceled), cancellationToken);
    }
}
