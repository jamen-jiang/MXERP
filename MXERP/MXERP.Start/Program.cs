using MXERP.Adapter.Controllers;

var builder = WebApplication.CreateBuilder(args);

// 注册控制器，并显式纳入 Adapter 层的 ApplicationPart
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(PingController).Assembly);

var app = builder.Build();

app.MapGet("/", () => "MXERP is running.");
app.MapControllers();

app.Run();
