using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Users;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 用户仓储实现, 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class UserRepository : EfCoreRepository<MXERPDbContext, User>, IUserRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public UserRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<User>> GetListByPostIdsAsync(IReadOnlyCollection<long> postIds, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).Where(x => x.UserPosts.Any(p => postIds.Contains(p.PostId))).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByUserNameExcludeIdAsync(string userName, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.UserName == userName && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByEmailExcludeIdAsync(string email, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Email == email && x.Id != id, cancellationToken);
    }
}
