using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作日志
/// </summary>
public class WorkLog : DeletionAuditedEntity
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected WorkLog()
    {
        TaskName = string.Empty;
        HandlerName = string.Empty;
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="taskName">任务名称</param>
    /// <param name="handlerId">处理人Id</param>
    /// <param name="handlerName">处理人名称</param>
    /// <param name="status">审批状态</param>
    /// <param name="remark">备注(可空)</param>
    internal WorkLog(string taskName, long handlerId, string handlerName, UserTaskStatusEnum status, string? remark)
    {
        TaskName = taskName;
        HandlerId = handlerId;
        HandlerName = handlerName;
        Status = status;
        Remark = remark;
        HandledOn = DateTime.Now;
    }

    /// <summary>
    /// 工作Id
    /// </summary>
    public long? WorkId { get; private set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; private set; }

    /// <summary>
    /// 处理人Id
    /// </summary>
    public long HandlerId { get; private set; }

    /// <summary>
    /// 处理人名称
    /// </summary>
    public string HandlerName { get; private set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    public UserTaskStatusEnum Status { get; private set; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 处理时间
    /// </summary>
    public DateTime? HandledOn { get; private set; }
}
