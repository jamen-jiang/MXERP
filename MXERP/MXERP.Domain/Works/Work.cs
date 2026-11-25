using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作
/// </summary>
public class Work : AuditedAggregateRoot
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected Work()
    {
        WorkflowId = string.Empty;
        Name = string.Empty;
        BusinessType = string.Empty;
        BusinessNumber = string.Empty;
        Status = string.Empty;
        WorkLogs = new List<WorkLog>();
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="workflowId">工作流Id</param>
    /// <param name="name">工作名称</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="businessId">业务Id</param>
    /// <param name="businessNumber">业务编号</param>
    public Work(string workflowId, string name, string businessType, long businessId, string businessNumber)
    {
        ValidateBusinessType(businessType);
        WorkflowId = workflowId;
        Name = name;
        BusinessType = businessType;
        BusinessId = businessId;
        BusinessNumber = businessNumber;
        Status = WorkStatusEnum.Running;
        WorkLogs = new List<WorkLog>();
    }

    /// <summary>
    /// 工作流Id
    /// </summary>
    public string WorkflowId { get; private set; }

    /// <summary>
    /// 工作名称
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public string BusinessType { get; private set; }

    /// <summary>
    /// 业务Id
    /// </summary>
    public long BusinessId { get; private set; }

    /// <summary>
    /// 业务编号
    /// </summary>
    public string BusinessNumber { get; private set; }

    /// <summary>
    /// 当前任务(可空)
    /// </summary>
    public string? CurrentTask { get; private set; }

    /// <summary>
    /// 当前任务分配人(可空)
    /// </summary>
    public string? CurrentTaskAssigner { get; private set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; private set; }

    /// <summary>
    /// 完成时间(可空)
    /// </summary>
    public DateTime? FinishedOn { get; private set; }

    /// <summary>
    /// 工作日志
    /// </summary>
    public virtual ICollection<WorkLog> WorkLogs { get; private set; }

    /// <summary>
    /// 获取工作日志列表(只读)
    /// </summary>
    /// <returns>不可变视图</returns>
    public IReadOnlyList<WorkLog> GetWorkLogs()
    {
        return WorkLogs.ToList().AsReadOnly();
    }

    /// <summary>
    /// 设置当前任务和审批人
    /// </summary>
    /// <param name="currentTask">当前任务</param>
    /// <param name="currentTaskAssigner">当前任务分配人</param>
    public void Modify(string currentTask, string currentTaskAssigner)
    {
        ValidateCurrentTask(currentTask);
        CurrentTask = currentTask;
        CurrentTaskAssigner = currentTaskAssigner;
    }

    /// <summary>
    /// 完成
    /// </summary>
    public void Finish()
    {
        CurrentTask = null;
        CurrentTaskAssigner = null;
        Status = WorkStatusEnum.Finished;
        FinishedOn = DateTime.Now;
    }

    /// <summary>
    /// 添加日志
    /// </summary>
    /// <param name="taskName">任务名称</param>
    /// <param name="assigneeId">处理人Id</param>
    /// <param name="assigneeName">处理人名称</param>
    /// <param name="status">审批状态</param>
    /// <param name="remark">备注(可空)</param>
    public void AddLog(string taskName, long assigneeId, string assigneeName, string status, string? remark)
    {
        ValidateCurrentTask(status);
        CurrentTask = null;
        CurrentTaskAssigner = null;
        WorkLog workLog = new WorkLog(taskName, assigneeId, assigneeName, status, remark);
        WorkLogs.Add(workLog);
        AddDomainEvent(new WorkLogAddedEvent(this, taskName, status));
    }

    /// <summary>
    /// 删除
    /// </summary>
    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new WorkDeletedEvent(this));
    }

    /// <summary>
    /// 校验业务类型
    /// </summary>
    /// <param name="businessType">业务类型</param>
    private static void ValidateBusinessType(string businessType)
    {
        if (string.IsNullOrWhiteSpace(businessType))
        {
            throw new UserFriendlyException("业务类型不能为空");
        }

        if (!WorkBusinessTypeEnum.IsValid(businessType))
        {
            throw new UserFriendlyException("业务类型" + businessType + "有误!");
        }
    }

    /// <summary>
    /// 校验当前任务
    /// </summary>
    /// <param name="currentTask">当前任务(可空)</param>
    private static void ValidateCurrentTask(string? currentTask)
    {
        if (currentTask is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(currentTask))
        {
            return;
        }

        if (!TaskEnum.IsValid(currentTask))
        {
            throw new UserFriendlyException("当前任务" + currentTask + "有误!");
        }
    }
}
