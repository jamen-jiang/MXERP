using Microsoft.EntityFrameworkCore;
using MXERP.Domain.Works;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 工作仓储实现
/// </summary>
[ScopedService]
public class WorkRepository : EfCoreRepository<MXERPDbContext, Work>, IWorkRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">数据库上下文提供器</param>
    public WorkRepository(IDbContextProvider<MXERPDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    /// <inheritdoc />
    public async Task<Work?> GetByWorkflowIdAsync(string workflowId, CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Include(x => x.WorkLogs)
            .FirstOrDefaultAsync(x => x.WorkflowId == workflowId, cancellationToken);
    }
}
