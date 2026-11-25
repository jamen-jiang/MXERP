using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Apps;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 应用仓储实现(对应 Java 的 AppRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class AppRepository : EfCoreRepository<MXERPDbContext, App>, IAppRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public AppRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<App>> GetWithChildrenAsync(long id, CancellationToken cancellationToken = default)
    {
        string idStr = id.ToString();
        return await (await GetQueryableAsync())
            .Where(x => x.Id == id
                || x.Level == idStr
                || x.Level!.StartsWith(idStr + ",")
                || x.Level!.EndsWith("," + idStr)
                || x.Level!.Contains("," + idStr + ","))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<App>> GetByParentIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.ParentId == parentId)
            .ToListAsync(cancellationToken);
    }
}
