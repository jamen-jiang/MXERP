using Microsoft.EntityFrameworkCore;
using MXERP.Domain.PurchaseOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 采购单仓储
/// </summary>
[ScopedService]
public class PurchaseOrderRepository : EfCoreRepository<MXERPDbContext, PurchaseOrder>, IPurchaseOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PurchaseOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<PurchaseOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        if (idList.Count == 0)
        {
            return new List<PurchaseOrder>();
        }

        return await (await GetQueryableAsync())
            .Include(x => x.PurchaseOrderItems)
            .ThenInclude(x => x.PurchaseOrderRelations)
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PurchaseOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.PurchaseOrderItems)
            .ThenInclude(x => x.PurchaseOrderRelations)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
