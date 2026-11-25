using MXERP.Infrastructure;
using MXERP.Web;
using Snow.Aop.Castle;
using Snow.AspNetCore;
using Snow.AspNetCore.Mvc;
using Snow.AspNetCore.RateLimiting;
using Snow.AspNetCore.Serilog;
using Snow.AspNetCore.SignalR;
using Snow.Authentication;
using Snow.Authorization;
using Snow.BackgroundJobs;
using Snow.BackgroundJobs.Hangfire;
using Snow.Caching.StackExchangeRedis;
using Snow.Data;
using Snow.DistributedLocking;
using Snow.Emailing.MailKit;
using Snow.EntityFrameworkCore;
using Snow.EventBus;
using Snow.Ftping.FluentFTP;
using Snow.Http.Client;
using Snow.Json;
using Snow.Localization;
using Snow.Messaging;
using Snow.Modularity;
using Snow.ObjectMapping.Mapster;
using Snow.OpenApi;
using Snow.Security;
using Snow.Timing;
using Snow.Tracing;
using Snow.Uow;
using Snow.Validation.FluentValidation;
using Elsa.Extensions;
using Elsa.Persistence.Dapper.Extensions;
using Elsa.Persistence.Dapper.Services;
using FluentMigrator.Runner;using Snow.Workflows.Elsa;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MXERP;

/// <summary>
/// MXERP 宿主模块: 一次性 <c>DependsOn</c> 框架全部能力模块与各实现提供者(与 Snow.Sample.Web 的 SampleModule 同形)。
/// <para>按 Snow 约定, 管道由宿主模块在 <see cref="OnApplicationInitialization"/> 里手写。</para>
/// </summary>
[DependsOn(
    typeof(WebModule),
    typeof(InfrastructureModule),
    typeof(SnowAopModule),
    typeof(SnowAspNetCoreModule),
    typeof(SnowAspNetCoreMvcModule),
    typeof(SnowRateLimitingModule),
    typeof(SnowAspNetCoreSerilogModule),
    typeof(SnowSignalRModule),
    typeof(SnowAuthenticationModule),
    typeof(SnowAuthorizationModule),
    typeof(BackgroundJobsModule),
    typeof(SnowHangfireModule),
    typeof(SnowCachingStackExchangeRedisModule),
    typeof(SnowDataModule),
    typeof(SnowDistributedLockRedisModule),
    typeof(SnowEmailingMailKitModule),
    typeof(SnowEntityFrameworkCoreModule),
    typeof(SnowEntityFrameworkCoreSqlServerModule),
    typeof(SnowEventBusModule),
    typeof(SnowFtpingFluentFTPModule),
    typeof(SnowHttpClientModule),
    typeof(SnowJsonModule),
    typeof(SnowLocalizationModule),
    typeof(SnowMessagingModule),
    typeof(SnowObjectMappingMapsterModule),
    typeof(SnowOpenApiModule),
    typeof(SnowSecurityModule),
    typeof(SnowTimingModule),
    typeof(SnowTracingModule),
    typeof(SnowUowModule),
    typeof(SnowValidationFluentValidationModule),
    typeof(SnowElsaModule)
)]
public sealed class MXERPModule : SnowModule
{
    /// <inheritdoc />
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Elsa 选项不读配置节(与 ABP 一致): 选项全由宿主用代码给, 照 Snow.Sample.Web/SampleModule 的写法。
        string elsaDbPath = Path.Combine(
            context.Services.GetHostingEnvironment().ContentRootPath,
            context.Configuration.GetConnectionString("Sqlite")
                ?? throw new InvalidOperationException("缺少配置 ConnectionStrings:Sqlite: Elsa 需要持久化存储的位置(文件路径)。"));
        Directory.CreateDirectory(Path.GetDirectoryName(elsaDbPath)!);
        string elsaConnectionString = $"Data Source={elsaDbPath}";

        Configure<SnowElsaOptions>(options =>
        {
            // ① 持久化: Elsa 3.10.2 的 EF 存储调 EF Core 10 的 API, 与基座的 EF Core 11 不兼容(GetQueryFilter 已删),
            //    所以这里留空让框架别挂 EF, 改由 Dapper 接管(见下方 ConfigureElsa)。
            options.ConnectionString = null;

            // ② 工作流 HTTP 端点: 关掉时管理 API 仍在。
            options.EnableHttp = true;
            options.HttpBasePath = "/workflows";

            // ③ 管理 API(/elsa/api): 工作流定义的增删改查与手动触发。
            options.EnableApi = true;

            // ④ 代码方式定义的工作流: 有工作流类时逐个 options.WorkflowTypes.Add(typeof(XxxWorkflow));
            //    MXERP 目前还没有工作流定义, 留空。

            // ⑤ 直接拿 Elsa 的 IModule 追加特性(内置之后补一刀)。
            options.ConfigureElsa = elsa =>
            {
                // 管理与运行时都换成 Dapper 存储(框架默认挂的是 EF Core)。
                elsa.UseWorkflowManagement(management => management.UseDapper(_ => { }));
                elsa.UseWorkflowRuntime(runtime => runtime.UseDapper(_ => { }));

                // Dapper 存储: 连接提供者 + 启动即迁移(表结构与包版本对不上时先建/补)。
                elsa.UseDapper(dapper =>
                {
                    dapper.DbConnectionProvider = _ => new SqliteDbConnectionProvider(elsaConnectionString);
                    dapper.UseMigrations(migrations => migrations.ConfigureRunner = runner => runner
                        .AddSQLite()
                        .WithGlobalConnectionString(elsaConnectionString)
                        .WithMigrationsIn(typeof(Elsa.Persistence.Dapper.Migrations.Identity.Initial).Assembly));
                });

                // 身份(Identity): 一样是代码写法。
                elsa.UseIdentity(identity =>
                {
                    identity.UseDapper(_ => { });

                    // 管理员账号: 每个租户激活时按用户名/密码/角色建一次(幂等)。
                    // 用 UseAdminUserProvider 只是内存里的静态账号, Dapper 存储下库里没人, 登录仍然不通。
                    identity.UseDefaultAdmin("admin", "Admin@123456", "admin");

                    // 令牌: 签名密钥不配就用 Elsa 的默认值, 这里用代码改有效期。
                    identity.TokenOptions = token =>
                    {
                        token.AccessTokenLifetime = TimeSpan.FromHours(8);
                        token.RefreshTokenLifetime = TimeSpan.FromDays(7);
                    };
                });
            };
        });
    }

    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // 认证选项: 框架不读配置节, 由宿主绑定(与 Snow.Sample.Web 同形)。
        Configure<SnowAuthenticationOptions>(context.Configuration.GetSection("Authentication"));

        // Hangfire 的存储由宿主选: 框架不绑定任何存储库(与 ABP 一样), 用 Hangfire 自己的 AddHangfire 追加。
        // 模块已经给过兼容级别与序列化器这些设置, 这里后加的存储排在后面, 持久化存储就盖掉默认值。
        // 换库换包与 UseXxxStorage 即可: Hangfire.SqlServer / Hangfire.PostgreSql / Hangfire.MySql。
        // 注意: SQLite 提供程序把这个参数当"数据库文件名"(不是连接串), 相对路径还按进程工作目录解析 ——
        // 从 IIS / 计划任务启动时库文件就跑到别处去了。以内容根为基准拼一次即可: 配成绝对路径时 Path.Combine 原样返回。
        string hangfireDbPath = Path.Combine(
            context.Services.GetHostingEnvironment().ContentRootPath,
            context.Configuration.GetConnectionString("Sqlite")
                ?? throw new InvalidOperationException("缺少配置 ConnectionStrings:Sqlite: Hangfire 需要持久化存储的位置(文件路径)。"));
        Directory.CreateDirectory(Path.GetDirectoryName(hangfireDbPath)!);
        context.Services.AddHangfire(configuration => configuration.UseSQLiteStorage(hangfireDbPath));
    }


    /// <inheritdoc />
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Snow 约定: 模块不铺管道, 整条管道由宿主模块在这里按行序手写(对齐 ABP 模板)。
        // 下面把框架各能力包提供的 UseSnowXxx 逐一接上, 书写顺序即中间件的嵌套顺序(先挂的在最外层)。
        IApplicationBuilder app = context.GetApplicationBuilder();

        // 0) 异常处理: 最外层, 把后续管道抛出的所有异常翻译成统一错误信封(JSON)
        app.UseSnowExceptionHandling();

        // 1) 链路标识: 必须最外层 —— 认证失败 / 授权拒绝 / 路由没匹配(404) 的响应也要带回 X-Correlation-Id
        app.UseSnowCorrelationId();

        // 2) 请求级日志上下文增强: 尽量靠外, 让后续中间件与业务日志都带上 用户 / 租户 / 链路;
        //    要能读到链路标识, 所以排在 UseSnowCorrelationId 之内
        app.UseSnowSerilogEnrichers();

        // 3) 登录态异常转 401/403: 必须在 UseAuthentication() 之前, 才能包住 JwtBearer 认证阶段抛出的令牌异常
        app.UseSnowTokenExceptionHandling();

        // 4) 路由: 后续限流与端点映射都依赖它先把 endpoint 解析出来
        app.UseRouting();

        // 5) 限流: 必须在 UseRouting() 之后 —— 限流策略挂在端点元数据上, 路由之前读不到会静默失效
        app.UseSnowRateLimiting();

        // 6) 认证
        app.UseAuthentication();

        // 7) 授权
        app.UseAuthorization();

        // 8) 后台作业仪表盘(/hangfire): 授权之后、端点之前 —— 终端分支, 早于授权就对匿名请求可见
        app.UseSnowHangfireDashboard();

        // 9) 工作流 HTTP 端点(/workflows): 同上, 终端分支, 落点必须在授权之后
        app.UseSnowWorkflows();

        // 10) 端点映射: 管道最后一步, 统一展开各模块登记的端点(含 OpenAPI / Scalar 文档)
        app.UseSnowEndpoints();
    }
}
