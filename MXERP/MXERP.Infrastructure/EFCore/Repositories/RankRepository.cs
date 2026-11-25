using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Ranks;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 职级仓储实现(对应 Java RankRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class RankRepository : EfCoreRepository<MXERPDbContext, Rank>, IRankRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public RankRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<Rank>> GetWithChildrenAsync(long id, CancellationToken cancellationToken = default)
    {
        string idStr = id.ToString();
        return await (await GetQueryableAsync())
            .Where(x => x.Id == id
                || (x.Level != null && (x.Level == idStr || x.Level.StartsWith(idStr + ",") || x.Level.EndsWith("," + idStr) || x.Level.Contains("," + idStr + ","))))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Rank>> GetByParentIdAsync(long parentId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.ParentId == parentId).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistAsync(string name, long? parentId, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.ParentId == parentId && x.Id != id, cancellationToken);
    }
}
