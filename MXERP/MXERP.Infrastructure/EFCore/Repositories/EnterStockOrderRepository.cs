using Microsoft.EntityFrameworkCore;
using MXERP.Domain.EnterStockOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 入库单仓储
/// </summary>
[ScopedService]
public class EnterStockOrderRepository : EfCoreRepository<MXERPDbContext, EnterStockOrder>, IEnterStockOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供者</param>
    public EnterStockOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.EnterStockOrderItems)
            .ThenInclude(x => x.EnterStockOrderRelations)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<EnterStockOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        return await (await GetQueryableAsync())
            .Include(x => x.EnterStockOrderItems)
            .ThenInclude(x => x.EnterStockOrderRelations)
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.EnterStockOrderItems)
            .ThenInclude(x => x.EnterStockOrderRelations)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
