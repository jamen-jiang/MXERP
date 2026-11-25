using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Stocks;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 库存仓储
/// </summary>
[ScopedService]
public class StockRepository : EfCoreRepository<MXERPDbContext, Stock>, IStockRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public StockRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Stock?> GetBySkuIdAndWarehouseIdAsync(long skuId, long warehouseId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.SkuId == skuId && x.WarehouseId == warehouseId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Stock>> GetListBySkuIdAsync(long skuId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.SkuId == skuId).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistBySkuIdAndWarehouseIdAsync(long skuId, long warehouseId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.SkuId == skuId && x.WarehouseId == warehouseId, cancellationToken);
    }
}
