namespace MXERP.Domain.Privileges;

/// <summary>
/// 权限通道(对应 Java 的 AccessEnum, 以编码字符串存储)
/// </summary>
public static class AccessEnum
{
    /// <summary>
    /// 应用
    /// </summary>
    public const string App = "App";
    /// <summary>
    /// 资源
    /// </summary>
    public const string Resource = "Resource";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        App, Resource
    };

    /// <summary>
    /// 编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        return code is not null && All.Contains(code);
    }
}
