using MXERP.Domain.Common;

namespace MXERP.Domain.CommandLogs;

/// <summary>
/// 命令日志
/// </summary>
public class CommandLog : LogAggregateRoot
{
    /// <summary>
    /// TraceId(可空)
    /// </summary>
    public string? TraceId { get; private set; }

    /// <summary>
    /// 命令名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 命令类型名称
    /// </summary>
    public string TypeName { get; private set; } = string.Empty;

    /// <summary>
    /// 无参构造器(供 EF 使用)
    /// </summary>
    protected CommandLog()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="traceId">TraceId(可空)</param>
    /// <param name="userId">用户Id(可空)</param>
    /// <param name="userName">用户名称(可空)</param>
    /// <param name="startedOn">开始时间</param>
    /// <param name="endedOn">结束时间</param>
    /// <param name="elapsedMilliseconds">耗时(毫秒)</param>
    /// <param name="name">命令名称</param>
    /// <param name="typeName">命令类型名称</param>
    /// <param name="request">请求数据(可空)</param>
    /// <param name="response">返回数据(可空)</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误信息(可空)</param>
    /// <param name="exception">异常信息(可空)</param>
    public CommandLog(string? traceId, long? userId, string? userName, DateTime startedOn, DateTime endedOn, long elapsedMilliseconds, string name, string typeName, string? request, string? response, bool isSuccess, string? errorMessage, string? exception)
        : base(userId, userName, startedOn, endedOn, elapsedMilliseconds, request, response, isSuccess, errorMessage, exception)
    {
        TraceId = traceId;
        Name = name;
        TypeName = typeName;
    }
}
