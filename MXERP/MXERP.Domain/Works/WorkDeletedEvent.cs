using Snow.Domain.Events;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作删除事件
/// </summary>
public class WorkDeletedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="work">工作</param>
    public WorkDeletedEvent(Work work) : base(work)
    {
        Work = work;
    }

    /// <summary>
    /// 工作
    /// </summary>
    public Work Work { get; }
}
