using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Dictionaries;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 字典仓储实现
/// </summary>
[ScopedService]
public class DictionaryRepository : EfCoreRepository<MXERPDbContext, Dictionary>, IDictionaryRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public DictionaryRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByCategoryExcludeIdAsync(string category, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Category == category && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }
}
