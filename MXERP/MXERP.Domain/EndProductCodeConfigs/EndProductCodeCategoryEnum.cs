namespace MXERP.Domain.EndProductCodeConfigs;

/// <summary>
/// 成品代码分类
/// </summary>
public static class EndProductCodeCategoryEnum
{
    /// <summary>
    /// 总计
    /// </summary>
    public const string Active = "Total";

    /// <summary>
    /// 停用
    /// </summary>
    public const string Inactive = "Inactive";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Active,
        Inactive
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
