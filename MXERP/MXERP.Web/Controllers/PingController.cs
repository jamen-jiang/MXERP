using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MXERP.Web.Controllers;

/// <summary>Web 层示例接口，用于验证管线连通。</summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "ok",
        layer = "web",
        timestamp = DateTimeOffset.UtcNow
    });
}
