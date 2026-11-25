using MXERP.Domain.Common;
using Snow.Exceptions;

namespace MXERP.Domain.SignInLogs;

/// <summary>
/// 登录日志
/// </summary>
public class SignInLog : LogAggregateRoot
{
    /// <summary>
    /// 登录类型
    /// </summary>
    public string Type { get; private set; } = string.Empty;

    /// <summary>
    /// 登录账号(可空)
    /// </summary>
    public string? UserCode { get; private set; }

    /// <summary>
    /// 令牌(可空)
    /// </summary>
    public string? Token { get; private set; }

    /// <summary>
    /// 刷新令牌(可空)
    /// </summary>
    public string? RefreshToken { get; private set; }

    /// <summary>
    /// 旧刷新令牌(可空)
    /// </summary>
    public string? OldRefreshToken { get; private set; }

    /// <summary>
    /// IP地址(可空)
    /// </summary>
    public string? Ip { get; private set; }

    /// <summary>
    /// 客户端地点(可空)
    /// </summary>
    public string? City { get; private set; }

    /// <summary>
    /// 客户端 UserAgent(可空)
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// 浏览器(可空)
    /// </summary>
    public string? Browser { get; private set; }

    /// <summary>
    /// 操作系统(可空)
    /// </summary>
    public string? Os { get; private set; }

    /// <summary>
    /// 无参构造器(供 EF 使用)
    /// </summary>
    protected SignInLog()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="type">登录类型</param>
    /// <param name="userId">用户Id(可空)</param>
    /// <param name="userCode">登录账号(可空)</param>
    /// <param name="userName">用户名称(可空)</param>
    /// <param name="startedOn">开始时间</param>
    /// <param name="endedOn">结束时间</param>
    /// <param name="elapsedMilliseconds">耗时(毫秒)</param>
    /// <param name="token">令牌(可空)</param>
    /// <param name="refreshToken">刷新令牌(可空)</param>
    /// <param name="oldRefreshToken">旧刷新令牌(可空)</param>
    /// <param name="ip">IP地址(可空)</param>
    /// <param name="city">客户端地点(可空)</param>
    /// <param name="userAgent">客户端 UserAgent(可空)</param>
    /// <param name="browser">浏览器(可空)</param>
    /// <param name="os">操作系统(可空)</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误信息(可空)</param>
    /// <param name="exception">异常信息(可空)</param>
    public SignInLog(string type, long? userId, string? userCode, string? userName, DateTime startedOn, DateTime endedOn, long elapsedMilliseconds, string? token, string? refreshToken, string? oldRefreshToken, string? ip, string? city, string? userAgent, string? browser, string? os, bool isSuccess, string? errorMessage, string? exception)
        : base(userId, userName, startedOn, endedOn, elapsedMilliseconds, null, null, isSuccess, errorMessage, exception)
    {
        Type = ValidateType(type);
        UserCode = userCode;
        Token = token;
        RefreshToken = refreshToken;
        OldRefreshToken = oldRefreshToken;
        Ip = ip;
        City = city;
        UserAgent = userAgent;
        Browser = browser;
        Os = os;
    }

    /// <summary>
    /// 校验登录类型
    /// </summary>
    /// <param name="type">登录类型</param>
    /// <returns>校验并 trim 后的类型</returns>
    private static string ValidateType(string type)
    {
        string? trimmed = type?.Trim();
        if (trimmed is null || trimmed.Length == 0 || !SignEntryTypeEnum.IsValid(trimmed))
        {
            throw new UserFriendlyException($"登录类型{type}有误!");
        }

        return trimmed;
    }
}
