namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 日期类型(对应 Java 的 DateTypeEnum, 以编码字符串存储)
/// </summary>
public static class DateTypeEnum
{
    /// <summary>
    /// 年
    /// </summary>
    public const string Year = "Year";

    /// <summary>
    /// 月
    /// </summary>
    public const string Month = "Month";

    /// <summary>
    /// 日
    /// </summary>
    public const string Day = "Day";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Year, Month, Day
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
