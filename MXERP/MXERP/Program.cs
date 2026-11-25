// MXERP 启动入口: 与 Snow.Sample.Web 同形 —— 建宿主 → 装配启动模块 → 驱动生命周期 → 跑。
// 服务注册(Hangfire 存储、Elsa 选项)全在 MXERPModule 的生命周期里做, Program 只负责装配与跑。
using Microsoft.EntityFrameworkCore;
using MXERP;
using MXERP.Infrastructure.EFCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
