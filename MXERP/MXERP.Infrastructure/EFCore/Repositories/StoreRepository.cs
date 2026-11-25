using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Stores;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 门店仓储实现, 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class StoreRepository : EfCoreRepository<MXERPDbContext, Store>, IStoreRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public StoreRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Store?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Store>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Store>> GetByListCodesAsync(IReadOnlyCollection<string> codes, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => codes.Contains(x.Code)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Code == code && x.Id != id, cancellationToken);
    }
}
