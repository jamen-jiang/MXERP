using Snow.Domain.Entities;

namespace MXERP.Domain.SystemLogs;

/// <summary>
/// 系统日志
/// </summary>
public class SystemLog : AggregateRoot
{
    /// <summary>
    /// 级别(可空)
    /// </summary>
    public string? Level { get; private set; }

    /// <summary>
    /// 错误信息(可空)
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// 消息模板(可空)
    /// </summary>
    public string? MessageTemplate { get; private set; }

    /// <summary>
    /// 异常信息(可空)
    /// </summary>
    public string? Exception { get; private set; }

    /// <summary>
    /// 属性(可空)
    /// </summary>
    public string? Properties { get; private set; }

    /// <summary>
    /// 记录时间(可空)
    /// </summary>
    public DateTime? TimeStamp { get; private set; }
}
