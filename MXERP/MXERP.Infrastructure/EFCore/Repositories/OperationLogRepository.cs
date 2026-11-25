using Microsoft.EntityFrameworkCore;
using MXERP.Domain.OperationLogs;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// OperationLog仓储实现(对应 Java 仓储), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class OperationLogRepository : EfCoreRepository<MXERPDbContext, OperationLog>, IOperationLogRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public OperationLogRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
