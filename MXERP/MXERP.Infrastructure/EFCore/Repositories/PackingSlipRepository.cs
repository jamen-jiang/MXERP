using Microsoft.EntityFrameworkCore;
using MXERP.Domain.PackingSlips;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 装箱单仓储
/// </summary>
[ScopedService]
public class PackingSlipRepository : EfCoreRepository<MXERPDbContext, PackingSlip>, IPackingSlipRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PackingSlipRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<int?> GetMaxCartonNoAsync(long storeId, CancellationToken cancellationToken = default)
    {
        IQueryable<PackingSlip> queryable = await GetQueryableAsync();
        return await queryable.Where(x => x.StoreId == storeId).MaxAsync(x => (int?)x.CartonNo, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PackingSlip?> GetByCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.CustomerOrderNumber == customerOrderNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PackingSlip?> GetByBrandCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.CustomerOrderNumber == customerOrderNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PackingSlip?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistCustomerOrderNumberAsync(string customerOrderNumber, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.CustomerOrderNumber == customerOrderNumber, cancellationToken);
    }
}
