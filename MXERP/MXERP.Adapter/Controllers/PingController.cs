using Microsoft.AspNetCore.Mvc;

namespace MXERP.Adapter.Controllers;

/// <summary>Adapter 层示例接口，用于验证 Web 管线连通。</summary>
[ApiController]
[Route("api/[controller]")]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        layer = "adapter",
        timestamp = DateTimeOffset.UtcNow
    });
}
