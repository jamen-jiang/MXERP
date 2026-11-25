namespace MXERP.Domain.Companies;

/// <summary>
/// 公司区域(对应 Java 的 CompanyRegionEnum, 以编码字符串存储)
/// </summary>
public static class CompanyRegionEnum
{
    /// <summary>
    /// 国内
    /// </summary>
    public const string Domestic = "Domestic";
    /// <summary>
    /// 国际
    /// </summary>
    public const string International = "International";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Domestic, International
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
