using Snow.Domain.Events;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作日志添加事件
/// </summary>
public class WorkLogAddedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="work">工作聚合(作为事件来源)</param>
    /// <param name="taskName">任务名称</param>
    /// <param name="status">任务状态</param>
    public WorkLogAddedEvent(Work work, string taskName, UserTaskStatusEnum status) : base(work)
    {
        BusinessId = work.BusinessId;
        TaskName = taskName;
        Status = status;
    }

    /// <summary>
    /// 业务Id
    /// </summary>
    public long BusinessId { get; }

    /// <summary>
    /// 任务名称(Check / Approve)
    /// </summary>
    public string TaskName { get; }

    /// <summary>
    /// 任务状态(Approve / Refuse)
    /// </summary>
    public UserTaskStatusEnum Status { get; }
}
