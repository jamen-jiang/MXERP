using Snow.Modularity;

namespace MXERP.Web;

/// <summary>
/// Web 层模块: 只是加载锚点。
/// <para>Snow 的 MVC 模块会把每个模块所在程序集登记成 ApplicationPart 再扫控制器,
/// 所以本层只要有一个 <see cref="SnowModule"/> 子类, <c>Controllers/</c> 下的控制器就会被自动发现,
/// 宿主不必再写 <c>AddApplicationPart</c>。以后本层放的应用服务(标 <c>[TransientService]</c> 等)
/// 也会被自动暴露成 Controller。</para>
/// </summary>
public sealed class WebModule : SnowModule
{
}
