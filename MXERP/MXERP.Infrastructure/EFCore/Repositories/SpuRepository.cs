using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Spus;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 产品仓储实现
/// </summary>
[ScopedService]
public class SpuRepository : EfCoreRepository<MXERPDbContext, Spu>, ISpuRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SpuRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<Spu>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Spu?> GetByProductTypeAndCodeAsync(string productType, string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .FirstOrDefaultAsync(x => x.ProductType == productType && x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .AnyAsync(x => x.Code == code && x.Id != id, cancellationToken);
    }
}
