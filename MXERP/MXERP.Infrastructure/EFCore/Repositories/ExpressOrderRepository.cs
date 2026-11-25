using Microsoft.EntityFrameworkCore;
using MXERP.Domain.ExpressOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 快递单仓储
/// </summary>
[ScopedService]
public class ExpressOrderRepository : EfCoreRepository<MXERPDbContext, ExpressOrder>, IExpressOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public ExpressOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<ExpressOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNumberExcludeIdAsync(string number, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Number == number && x.Id != id, cancellationToken);
    }
}
