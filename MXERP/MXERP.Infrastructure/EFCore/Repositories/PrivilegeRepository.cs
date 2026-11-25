using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Privileges;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 权限仓储实现(对应 Java 的 PrivilegeRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class PrivilegeRepository : EfCoreRepository<MXERPDbContext, Privilege>, IPrivilegeRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public PrivilegeRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<List<Privilege>> GetListByMasterAsync(string master, long masterValue, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.Master == master && x.MasterValue == masterValue)
            .ToListAsync(cancellationToken);
    }
}
