using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 流水号仓储实现
/// </summary>
[ScopedService]
public class SerialNumberRepository : EfCoreRepository<MXERPDbContext, SerialNumber>, ISerialNumberRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SerialNumberRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<SerialNumber?> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Type == type, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByTypeExcludeIdAsync(string type, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Type == type && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByPrefixExcludeIdAsync(string prefix, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Prefix == prefix && x.Id != id, cancellationToken);
    }
}
