// MXERP 启动入口: 与 Snow.Sample.Web 同形 —— 建宿主 → 登记框架外围 → 装配启动模块 → 驱动生命周期 → 跑。
using Elsa.Extensions;
using FluentMigrator.Runner;
using Elsa.Persistence.Dapper.Extensions;
using Elsa.Persistence.Dapper.Services;
using Hangfire.Storage.SQLite;
using Microsoft.EntityFrameworkCore;
using MXERP;
using MXERP.Infrastructure.EFCore;
using Snow.Workflows.Elsa;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 应用级 SQLite 库文件: Hangfire 现在用, Elsa 的持久化也用同一个文件, 所以按通用名配置、在这里算一次。
// SQLite 只认库文件名(不是连接串), 相对路径还按进程工作目录解析 —— 从 IIS / 计划任务启动时
// 库文件就跑到别处去了。以内容根为基准拼一次即可: 配置写绝对路径时 Path.Combine 原样返回它。
string sqliteDbPath = Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration.GetConnectionString("Sqlite")
        ?? throw new InvalidOperationException("缺少配置 ConnectionStrings:Sqlite: SQLite 库文件位置。"));
Directory.CreateDirectory(Path.GetDirectoryName(sqliteDbPath)!);

// Hangfire 持久化存储: 用框架的 AddSnowHangfire 登记(兼容级别/序列化器/序列化设置这些样板已内置),
// 只需在回调里指定存储。换库换包与 UseXxxStorage 即可: Hangfire.SqlServer / Hangfire.PostgreSql / Hangfire.MySql。
builder.Services.AddSnowHangfire(configuration => configuration.UseSQLiteStorage(sqliteDbPath));

// SnowElsaModule 已内置运行工作流所需的最小特性集(表达式、JS 引擎、内存缓存、管理与运行时的缓存),
// 宿主只需补取舍项: 身份(播种一个默认管理员, 否则没有账号能登录)与持久化。
// 持久化走 Dapper 而不是 EF: Elsa 3.10.2 的 EF 存储调用 EF Core 10 的 API(GetQueryFilter 等),
// 基座是 EF Core 11 且 Snow 的 EF 包硬声明 11, 两者不能同进程; Dapper 版只用 Microsoft.Data.Sqlite, 不碰 EF。
string elsaConnectionString = $"Data Source={sqliteDbPath}";
builder.Services.AddSnowElsa(options =>
{
    options.ConfigureWorkflowManagement = management => management.UseDapper(_ => { });
    options.ConfigureWorkflowRuntime = runtime => runtime.UseDapper(_ => { });
    options.ConfigureElsa = elsa =>
    {
        elsa.UseDapper(dapper =>
        {
            dapper.DbConnectionProvider = _ => new SqliteDbConnectionProvider(elsaConnectionString);

            // 建表交给 FluentMigrator: Elsa 的 Dapper 存储不自带建表, 迁移特性必须显式指定数据库提供程序。
            dapper.UseMigrations(migrations => migrations.ConfigureRunner = runner => runner
                .AddSQLite()
                .WithGlobalConnectionString(elsaConnectionString)
                .WithMigrationsIn(typeof(Elsa.Persistence.Dapper.Migrations.Identity.Initial).Assembly));
        });

        elsa.UseIdentity(identity =>
        {
            identity.UseDapper(_ => { });

            // 播种一个默认管理员: 每个租户激活时幂等创建角色与用户, 否则管理端点匿名 401、有身份无权限 403。
            identity.UseDefaultAdmin("admin", "Admin@123456", "admin");
        });
    };
});

// 装配启动模块及其全部依赖, 并立即执行服务注册阶段
await builder.AddApplicationAsync<MXERPModule>();

WebApplication app = builder.Build();

// 驱动模块生命周期(必须 Build 之后、Run 之前调用一次)
await app.InitializeApplicationAsync();

// 阶段 0 样板: 开发环境自动建库建表(目标库 ConnectionStrings:Default = SQL Server MXERP)。
// 生产改为 EF Core 迁移(D1 迁移后续补), 不要用 EnsureCreated。
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    MXERPDbContext dbContext = scope.ServiceProvider.GetRequiredService<MXERPDbContext>();
    dbContext.Database.EnsureCreated();
}

// 端点由宿主映射(框架不代挂骨架, 与 ABP 模板风格一致)。
// 认证模块把全局回退策略设为「必须已登录」, 所以公开探针端点显式 AllowAnonymous。
app.MapGet("/", () => "MXERP is running.").AllowAnonymous();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

await app.RunAsync();
