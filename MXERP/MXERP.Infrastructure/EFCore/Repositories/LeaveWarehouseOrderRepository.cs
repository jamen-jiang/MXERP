using Microsoft.EntityFrameworkCore;
using MXERP.Domain.LeaveWarehouseOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 出仓单仓储
/// </summary>
[ScopedService]
public class LeaveWarehouseOrderRepository : EfCoreRepository<MXERPDbContext, LeaveWarehouseOrder>, ILeaveWarehouseOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public LeaveWarehouseOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<LeaveWarehouseOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.LeaveWarehouseOrderItems)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
