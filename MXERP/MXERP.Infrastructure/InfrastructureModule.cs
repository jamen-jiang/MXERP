using MXERP.Domain;
using Snow.EntityFrameworkCore;
using Snow.Modularity;

namespace MXERP.Infrastructure;

/// <summary>
/// 基础设施层模块: 加载 EF Core 与 SQL Server 提供程序, 并选定默认数据库提供程序。
/// <para>提供程序选择放在这里(而不是宿主): 想换库只改本模块一行 <c>UseSqlServer</c> → <c>UseSqlite</c> 等,
/// 领域与应用层完全无感。<c>SnowEntityFrameworkCoreModule</c> 会扫描本程序集, 自动登记
/// <c>MXERPDbContext</c> 与它每个 <c>DbSet</c> 对应的 <c>IRepository&lt;T&gt;</c>。</para>
/// </summary>
[DependsOn(typeof(DomainModule))]
[DependsOn(typeof(SnowEntityFrameworkCoreModule))]
[DependsOn(typeof(SnowEntityFrameworkCoreSqlServerModule))]
public sealed class InfrastructureModule : SnowModule
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<SnowDbContextOptions>(options => options.UseSqlServer());
    }
}
