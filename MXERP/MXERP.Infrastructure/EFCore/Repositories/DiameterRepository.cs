using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Diameters;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 直径仓储实现(对应 Java DiameterRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class DiameterRepository : EfCoreRepository<MXERPDbContext, Diameter>, IDiameterRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public DiameterRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Diameter?> GetBySizeAsync(int size, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Size == size, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Diameter?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.IsDefault, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistBySizeExcludeIdAsync(int size, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Size == size && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByColorExcludeIdAsync(string color, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Color == color && x.Id != id, cancellationToken);
    }
}
