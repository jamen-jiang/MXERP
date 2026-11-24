using Snow.Modularity;

namespace MXERP.Domain;

/// <summary>
/// 领域层模块: 只是加载锚点。
/// <para>Snow 的约定注册会扫描各模块所在程序集, 本层标了 <c>[ScopedService]</c> 的工厂/领域服务
/// 会被自动登记进容器; 宿主不必逐个手写 <c>AddScoped</c>。</para>
/// </summary>
public sealed class DomainModule : SnowModule
{
}
