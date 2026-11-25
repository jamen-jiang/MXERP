using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Jobs;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 任务仓储实现(对应 Java JobRepository), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class JobRepository : EfCoreRepository<MXERPDbContext, Job>, IJobRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public JobRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Job?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.Name == name && x.Id != id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsExistByBeanAndMethodExcludeIdAsync(string beanName, string methodName, long? id = null, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync()).AnyAsync(x => x.BeanName == beanName && x.MethodName == methodName && x.Id != id, cancellationToken);
    }
}
