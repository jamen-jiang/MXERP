using MXERP.Domain.ExternalOrderLogs;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// 外部订单日志仓储
/// </summary>
[ScopedService]
public class ExternalOrderLogRepository : EfCoreRepository<MXERPDbContext, ExternalOrderLog>, IExternalOrderLogRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public ExternalOrderLogRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
