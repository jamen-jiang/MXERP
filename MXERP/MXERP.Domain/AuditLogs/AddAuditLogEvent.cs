using Snow.Domain.Entities;
using Snow.Domain.Events;

namespace MXERP.Domain.AuditLogs;

/// <summary>
/// 添加审计日志事件(对应 Java AddAuditLogEvent)
/// </summary>
public class AddAuditLogEvent : DomainEvent
{
    /// <summary>
    /// 动作
    /// </summary>
    public string Action { get; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="aggregateRoot">聚合根</param>
    /// <param name="action">动作</param>
    /// <param name="remark">备注(可空)</param>
    public AddAuditLogEvent(IAggregateRoot aggregateRoot, string action, string? remark)
        : base(aggregateRoot)
    {
        Action = action;
        Remark = remark;
    }

    /// <summary>
    /// 构造器(备注为空)
    /// </summary>
    /// <param name="aggregateRoot">聚合根</param>
    /// <param name="action">动作</param>
    public AddAuditLogEvent(IAggregateRoot aggregateRoot, string action)
        : this(aggregateRoot, action, null)
    {
    }
}
