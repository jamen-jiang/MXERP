namespace MXERP.Domain.Companies;

/// <summary>
/// 公司组别(对应 Java 的 CompanyGroupEnum, 以编码字符串存储)
/// </summary>
public static class CompanyGroupEnum
{
    /// <summary>
    /// 海外
    /// </summary>
    public const string Overseas = "Overseas";
    /// <summary>
    /// 东南亚
    /// </summary>
    public const string Sea = "Sea";
    /// <summary>
    /// 其他分工厂
    /// </summary>
    public const string Other = "Other";
    /// <summary>
    /// 国内
    /// </summary>
    public const string Domestic = "Domestic";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Overseas, Sea, Other, Domestic
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
