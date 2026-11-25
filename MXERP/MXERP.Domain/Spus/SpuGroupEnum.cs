namespace MXERP.Domain.Spus;

/// <summary>
/// 组别
/// </summary>
public static class SpuGroupEnum
{
    /// <summary>
    /// 常用
    /// </summary>
    public const string Common = "Common";

    /// <summary>
    /// 不常用
    /// </summary>
    public const string Unusual = "Unusual";

    /// <summary>
    /// 呆滞
    /// </summary>
    public const string Dull = "Dull";

    /// <summary>
    /// 新增
    /// </summary>
    public const string New = "New";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Common,
        Unusual,
        Dull,
        New
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>是否有效</returns>
    public static bool IsValid(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && All.Contains(code);
    }
}
