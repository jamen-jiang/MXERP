namespace MXERP.Domain.SignInLogs;

/// <summary>
/// 登录入口类型(对应 Java 的 SignEntryTypeEnum, 以编码字符串存储)
/// </summary>
public static class SignEntryTypeEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    public const string SignIn = "SignIn";
    /// <summary>
    /// 刷新令牌
    /// </summary>
    public const string RefreshToken = "RefreshToken";
    /// <summary>
    /// 登出
    /// </summary>
    public const string SignOut = "SignOut";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        SignIn, RefreshToken, SignOut
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
