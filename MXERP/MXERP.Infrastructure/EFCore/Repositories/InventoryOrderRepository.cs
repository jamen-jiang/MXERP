using Microsoft.EntityFrameworkCore;
using MXERP.Domain.InventoryOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 盘点单仓储
/// </summary>
[ScopedService]
public class InventoryOrderRepository : EfCoreRepository<MXERPDbContext, InventoryOrder>, IInventoryOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public InventoryOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<InventoryOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.InventoryOrderItems)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
