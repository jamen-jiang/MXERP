using Microsoft.EntityFrameworkCore;
using MXERP.Domain.PurchaseRequisitionOrders;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 申购单仓储
/// </summary>
[ScopedService]
public class PurchaseRequisitionOrderRepository : EfCoreRepository<MXERPDbContext, PurchaseRequisitionOrder>, IPurchaseRequisitionOrderRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PurchaseRequisitionOrderRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<PurchaseRequisitionOrder>> GetListByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        List<long> idList = ids.ToList();
        if (idList.Count == 0)
        {
            return new List<PurchaseRequisitionOrder>();
        }

        return await (await GetQueryableAsync())
            .Include(x => x.PurchaseRequisitionOrderItems)
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PurchaseRequisitionOrder?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.PurchaseRequisitionOrderItems)
            .FirstOrDefaultAsync(x => x.Number == number, cancellationToken);
    }
}
