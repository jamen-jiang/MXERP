using Microsoft.EntityFrameworkCore;
using MXERP.Domain.LeaveStockOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 出库单仓储
/// </summary>
[ScopedService]
public class LeaveStockOrderRepository : EfCoreRepository<MXERPDbContext, LeaveStockOrder>, ILeaveStockOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供者</param>
    public LeaveStockOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<LeaveStockOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        return await (await GetQueryableAsync())
            .Include(x => x.LeaveStockOrderItems)
            .ThenInclude(x => x.LeaveStockOrderRelations)
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.LeaveStockOrderItems)
            .ThenInclude(x => x.LeaveStockOrderRelations)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
