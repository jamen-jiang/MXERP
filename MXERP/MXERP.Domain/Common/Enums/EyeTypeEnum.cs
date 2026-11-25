namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 眼别(对应 Java EyeTypeEnum, 以编码字符串存储)
/// </summary>
public static class EyeTypeEnum
{
    /// <summary>
    /// 双眼
    /// </summary>
    public const string RL = "RL";
    /// <summary>
    /// 右眼
    /// </summary>
    public const string R = "R";
    /// <summary>
    /// 左眼
    /// </summary>
    public const string L = "L";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        RL, R, L
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
