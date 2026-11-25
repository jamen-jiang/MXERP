using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SystemLogs;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// SystemLog仓储实现(对应 Java 仓储), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class SystemLogRepository : EfCoreRepository<MXERPDbContext, SystemLog>, ISystemLogRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SystemLogRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
