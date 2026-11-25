namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 度数类型
/// </summary>
public static class DegreesTypeEnum
{
    /// <summary>
    /// 正散
    /// </summary>
    public const string ZhengSan = "+/-";

    /// <summary>
    /// 负散
    /// </summary>
    public const string FuSan = "-/-";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        ZhengSan,
        FuSan
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
