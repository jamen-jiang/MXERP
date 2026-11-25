using Microsoft.EntityFrameworkCore;
using MXERP.Domain.ExternalLeaveStockOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 外部出库单仓储
/// </summary>
[ScopedService]
public class ExternalLeaveStockOrderRepository : EfCoreRepository<MXERPDbContext, ExternalLeaveStockOrder>, IExternalLeaveStockOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public ExternalLeaveStockOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<ExternalLeaveStockOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        IQueryable<ExternalLeaveStockOrder> queryable = await GetQueryableAsync();
        return await queryable
            .Include(x => x.ExternalLeaveStockOrderTrackings)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
