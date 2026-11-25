using Microsoft.EntityFrameworkCore;
using MXERP.Domain.SignInLogs;
using Snow.DependencyInjection;
using Snow.Domain.Repositories.EntityFrameworkCore;
using Snow.EntityFrameworkCore;

namespace MXERP.Infrastructure.EFCore.Repositories;

/// <summary>
/// SignInLog仓储实现(对应 Java 仓储), 复用框架 EfCoreRepository
/// </summary>
[ScopedService]
public class SignInLogRepository : EfCoreRepository<MXERPDbContext, SignInLog>, ISignInLogRepository
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="dbContextProvider">上下文提供器</param>
    public SignInLogRepository(IDbContextProvider<MXERPDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
