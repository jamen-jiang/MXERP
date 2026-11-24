// MXERP 启动入口: 与 Snow.Sample.Web 同形 —— 建宿主 → 登记框架外围 → 装配启动模块 → 驱动生命周期 → 跑。
using Elsa.Extensions;
using Hangfire.Storage.SQLite;
using MXERP.Start;
using Snow.Workflows.Elsa;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Hangfire 持久化存储: 用框架的 AddSnowHangfire 登记(兼容级别/序列化器/序列化设置这些样板已内置),
// 只需在回调里指定存储。换库换包与 UseXxxStorage 即可: Hangfire.SqlServer / Hangfire.PostgreSql / Hangfire.MySql。
// SQLite 提供程序既接受纯文件路径, 也接受连接串。两种写法里的相对路径都按内容根(项目目录)解析,
// 所以库文件位置只写在 appsettings 的 ConnectionStrings:Hangfire 里, 代码不重复一份。
string hangfireConnectionString = builder.Configuration.GetConnectionString("Hangfire")
    ?? throw new InvalidOperationException("缺少配置 ConnectionStrings:Hangfire: Hangfire 需要持久化存储的位置(文件路径或连接串)。");
builder.Services.AddSnowHangfire(configuration =>
    configuration.UseSQLiteStorage(ResolveSqlitePath(hangfireConnectionString, builder.Environment.ContentRootPath)));

// SnowElsaModule 已内置运行工作流所需的最小特性集(表达式、JS 引擎、内存缓存、管理与运行时的缓存),
// 宿主只需补取舍项。这里挂的是身份: 仅开发环境播种一个开发管理员, 免去「无用户可登录」的启动诊断。
builder.Services.AddSnowElsa(options =>
{
    options.ConfigureElsa = elsa => elsa.UseIdentity(identity => identity.UseDevelopmentAdminUserProvider());
});

// 装配启动模块及其全部依赖, 并立即执行服务注册阶段
await builder.AddApplicationAsync<StartModule>();

WebApplication app = builder.Build();

// 驱动模块生命周期(必须 Build 之后、Run 之前调用一次)
await app.InitializeApplicationAsync();

// 端点由宿主映射(框架不代挂骨架, 与 ABP 模板风格一致)。
// 认证模块把全局回退策略设为「必须已登录」, 所以公开探针端点显式 AllowAnonymous。
app.MapGet("/", () => "MXERP is running.").AllowAnonymous();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

await app.RunAsync();

/// <summary>
/// 把 SQLite 连接串/文件路径里的相对路径解析成绝对路径, 并保证目录存在。
/// <para>SQLite 提供程序两种写法都认: 纯文件路径(<c>DataBase/hangfire.db</c>)与连接串
/// (<c>Data Source=DataBase/hangfire.db;Cache=Shared</c>)。不处理的话, 相对路径会按进程工作目录解析,
/// 从 IIS / 计划任务启动时库文件就跑到别处去了。</para>
/// </summary>
/// <param name="connectionString">配置里的原始值。</param>
/// <param name="contentRootPath">内容根目录, 相对路径以它为基准。</param>
/// <returns>解析后的连接串或绝对路径。</returns>
static string ResolveSqlitePath(string connectionString, string contentRootPath)
{
    // 连接串形态: 只改写 Data Source / Filename 的值, 其余键原样保留
    int sourceIndex = connectionString.IndexOf("Data Source=", StringComparison.OrdinalIgnoreCase);
    if (sourceIndex < 0)
    {
        sourceIndex = connectionString.IndexOf("Filename=", StringComparison.OrdinalIgnoreCase);
    }

    if (sourceIndex >= 0)
    {
        int valueStart = connectionString.IndexOf('=', sourceIndex) + 1;
        int valueEnd = connectionString.IndexOf(';', valueStart);
        string path = valueEnd < 0
            ? connectionString[valueStart..]
            : connectionString[valueStart..valueEnd];

        string resolved = ResolveSqlitePath(path.Trim(), contentRootPath);
        return string.Concat(connectionString.AsSpan(0, valueStart), resolved, valueEnd < 0 ? string.Empty : connectionString[valueEnd..]);
    }

    // 纯文件路径形态(也覆盖 :memory: 这类特殊值: 它不是路径, 原样返回)
    if (connectionString is ":memory:" || connectionString.Length == 0 || Path.IsPathRooted(connectionString))
    {
        return connectionString;
    }

    string fullPath = Path.GetFullPath(Path.Combine(contentRootPath, connectionString));
    string? directory = Path.GetDirectoryName(fullPath);
    if (!string.IsNullOrEmpty(directory))
    {
        Directory.CreateDirectory(directory);
    }

    return fullPath;
}
