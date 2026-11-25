using Microsoft.EntityFrameworkCore;
using MXERP.Domain.EndProductCodeConfigs;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 成品代码配置仓储实现
/// </summary>
[ScopedService]
public class EndProductCodeConfigRepository : EfCoreRepository<MXERPDbContext, EndProductCodeConfig>, IEndProductCodeConfigRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public EndProductCodeConfigRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<EndProductCodeConfig?> GetBySpuIdAndDegreesTypeAsync(long spuId, string degreesType, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.SpuId == spuId && x.DegreesType == degreesType, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistBySpuIdAndDegreesTypeExcludeIdAsync(long spuId, string degreesType, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.SpuId == spuId && x.DegreesType == degreesType && x.Id != id, cancellationToken);
    }
}
