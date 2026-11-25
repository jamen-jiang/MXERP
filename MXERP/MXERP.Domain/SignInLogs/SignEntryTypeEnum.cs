using System.ComponentModel;

namespace MXERP.Domain.SignInLogs;

/// <summary>
/// 登录入口类型(对应 Java 的 SignEntryTypeEnum, 以编码字符串存储)
/// </summary>
public enum SignEntryTypeEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    [Description("登录")]
    SignIn,

    /// <summary>
    /// 刷新令牌
    /// </summary>
    [Description("刷新令牌")]
    RefreshToken,

    /// <summary>
    /// 登出
    /// </summary>
    [Description("登出")]
    SignOut
}
