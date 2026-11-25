using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.AuditLogs;

/// <summary>
/// 审计日志
/// </summary>
public class AuditLog : CreationAuditedAggregateRoot
{
    /// <summary>
    /// 实体类型
    /// </summary>
    public string EntityType { get; private set; } = string.Empty;

    /// <summary>
    /// 实体Id
    /// </summary>
    public long EntityId { get; private set; }

    /// <summary>
    /// 动作
    /// </summary>
    public string Action { get; private set; } = string.Empty;

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 无参构造器(供 EF 使用)
    /// </summary>
    protected AuditLog()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="entityId">实体Id</param>
    /// <param name="action">动作</param>
    /// <param name="remark">备注(可空)</param>
    public AuditLog(string entityType, long entityId, string action, string? remark)
    {
        if (entityType.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("实体类型不能为空");
        }

        if (action.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("动作不能为空");
        }

        EntityType = entityType.Trim();
        EntityId = entityId;
        Action = action.Trim();
        Remark = remark;
    }
}
