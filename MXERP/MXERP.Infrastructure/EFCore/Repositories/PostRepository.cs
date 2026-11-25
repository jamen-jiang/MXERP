using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Posts;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 岗位仓储实现, 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class PostRepository : EfCoreRepository<MXERPDbContext, Post>, IPostRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PostRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Post?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Post>> GetListByDepartmentIdsAsync(IReadOnlyCollection<long> departmentIds, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => departmentIds.Contains(x.DepartmentId)).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }
}
