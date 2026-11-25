using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Common;

/// <summary>
/// 日志聚合根基类(对应 Java 的 LogAggregateRoot)
/// </summary>
public abstract class LogAggregateRoot : DeletionAuditedAggregateRoot
{
    /// <summary>
    /// 用户Id(可空)
    /// </summary>
    public long? UserId { get; protected set; }

    /// <summary>
    /// 用户名称(可空)
    /// </summary>
    public string? UserName { get; protected set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartedOn { get; protected set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndedOn { get; protected set; }

    /// <summary>
    /// 耗时(毫秒)
    /// </summary>
    public long ElapsedMilliseconds { get; protected set; }

    /// <summary>
    /// 请求数据(可空)
    /// </summary>
    public string? Request { get; protected set; }

    /// <summary>
    /// 返回数据(可空)
    /// </summary>
    public string? Response { get; protected set; }

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; protected set; }

    /// <summary>
    /// 错误信息(可空)
    /// </summary>
    public string? ErrorMessage { get; protected set; }

    /// <summary>
    /// 异常信息(可空)
    /// </summary>
    public string? Exception { get; protected set; }

    /// <summary>
    /// 无参构造器(供 EF 使用)
    /// </summary>
    protected LogAggregateRoot()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="userId">用户Id(可空, 0 视为 null)</param>
    /// <param name="userName">用户名称(可空)</param>
    /// <param name="startedOn">开始时间</param>
    /// <param name="endedOn">结束时间</param>
    /// <param name="elapsedMilliseconds">耗时(毫秒)</param>
    /// <param name="request">请求数据(可空)</param>
    /// <param name="response">返回数据(可空)</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误信息(可空)</param>
    /// <param name="exception">异常信息(可空)</param>
    protected LogAggregateRoot(long? userId, string? userName, DateTime startedOn, DateTime endedOn, long elapsedMilliseconds, string? request, string? response, bool isSuccess, string? errorMessage, string? exception)
    {
        if (userId == 0)
        {
            UserId = null;
            UserName = null;
        }
        else
        {
            UserId = userId;
            UserName = userName;
        }

        StartedOn = startedOn;
        EndedOn = endedOn;
        ElapsedMilliseconds = elapsedMilliseconds;
        Request = request;
        Response = response;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }
}
