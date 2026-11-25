using Microsoft.EntityFrameworkCore;
using MXERP.Domain.OrderEvents;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 订单事件仓储
/// </summary>
[ScopedService]
public class OrderEventRepository : EfCoreRepository<MXERPDbContext, OrderEvent>, IOrderEventRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public OrderEventRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<OrderEvent?> GetByOrderTypeAndOrderStatusAsync(string orderType, string orderStatus, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.OrderType == orderType && x.OrderStatus == orderStatus, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByOrderTypeAndOrderStatusExcludeIdAsync(string orderType, string orderStatus, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.OrderType == orderType && x.OrderStatus == orderStatus && x.Id != id, cancellationToken);
    }
}
