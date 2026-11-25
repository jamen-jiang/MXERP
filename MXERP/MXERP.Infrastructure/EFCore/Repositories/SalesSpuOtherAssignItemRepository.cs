using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SalesSpuOtherAssignItems;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 销售产品其他分配项仓储实现
/// </summary>
[ScopedService]
public class SalesSpuOtherAssignItemRepository : EfCoreRepository<MXERPDbContext, SalesSpuOtherAssignItem>, ISalesSpuOtherAssignItemRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SalesSpuOtherAssignItemRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<SalesSpuOtherAssignItem?> GetBySalesSpuIdAndCoatingAsync(long salesSpuId, string coating, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.SalesSpuId == salesSpuId && x.Coating == coating, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistBySalesSpuIdAndCoatingExcludeIdAsync(long salesSpuId, string coating, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.SalesSpuId == salesSpuId && x.Coating == coating && x.Id != id, cancellationToken);
    }
}
