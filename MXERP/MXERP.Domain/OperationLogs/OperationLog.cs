using MXERP.Domain.Common;

namespace MXERP.Domain.OperationLogs;

/// <summary>
/// 操作日志
/// </summary>
public class OperationLog : LogAggregateRoot
{
    /// <summary>
    /// TraceId
    /// </summary>
    public string TraceId { get; private set; } = string.Empty;

    /// <summary>
    /// 接口模块
    /// </summary>
    public string ApiModule { get; private set; } = string.Empty;

    /// <summary>
    /// 接口名称
    /// </summary>
    public string ApiName { get; private set; } = string.Empty;

    /// <summary>
    /// 接口地址
    /// </summary>
    public string ApiUrl { get; private set; } = string.Empty;

    /// <summary>
    /// 接口提交方法
    /// </summary>
    public string ApiMethod { get; private set; } = string.Empty;

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
    protected OperationLog()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="traceId">TraceId</param>
    /// <param name="userId">用户Id</param>
    /// <param name="userName">用户名称</param>
    /// <param name="startedOn">开始时间</param>
    /// <param name="endedOn">结束时间</param>
    /// <param name="elapsedMilliseconds">耗时(毫秒)</param>
    /// <param name="apiModule">接口模块</param>
    /// <param name="apiName">接口名称</param>
    /// <param name="apiUrl">接口地址</param>
    /// <param name="apiMethod">接口提交方法</param>
    /// <param name="ip">IP地址(可空)</param>
    /// <param name="city">客户端地点(可空)</param>
    /// <param name="userAgent">客户端 UserAgent(可空)</param>
    /// <param name="browser">浏览器(可空)</param>
    /// <param name="os">操作系统(可空)</param>
    /// <param name="request">请求数据(可空)</param>
    /// <param name="response">返回数据(可空)</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误信息(可空)</param>
    /// <param name="exception">异常信息(可空)</param>
    public OperationLog(string traceId, long? userId, string? userName, DateTime startedOn, DateTime endedOn, long elapsedMilliseconds, string apiModule, string apiName, string apiUrl, string apiMethod, string? ip, string? city, string? userAgent, string? browser, string? os, string? request, string? response, bool isSuccess, string? errorMessage, string? exception)
        : base(userId, userName, startedOn, endedOn, elapsedMilliseconds, request, response, isSuccess, errorMessage, exception)
    {
        TraceId = traceId;
        ApiModule = apiModule;
        ApiName = apiName;
        ApiUrl = apiUrl;
        ApiMethod = apiMethod;
        Ip = ip;
        City = city;
        UserAgent = userAgent;
        Browser = browser;
        Os = os;
    }
}
