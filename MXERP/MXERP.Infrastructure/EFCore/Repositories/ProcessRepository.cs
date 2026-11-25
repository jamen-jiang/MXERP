using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Processes;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 工序仓储实现
/// </summary>
[ScopedService]
public class ProcessRepository : EfCoreRepository<MXERPDbContext, Process>, IProcessRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public ProcessRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Process?> GetByTypeAndCodeAsync(string type, string code, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Type == type && x.Code == code, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByTypeAndCodeExcludeIdAsync(string type, string code, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Type == type && x.Code == code && x.Id != id, cancellationToken);
    }
}
