using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Units;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 单位仓储实现(对应 Java UnitRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class UnitRepository : EfCoreRepository<MXERPDbContext, Unit>, IUnitRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public UnitRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Unit?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Code == code && x.Id != id, cancellationToken);
    }
}
