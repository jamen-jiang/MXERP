using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Routings;

/// <summary>
/// 工艺路线
/// </summary>
public class Routing : AuditedAggregateRoot
{
    /// <summary>
    /// 路线代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 路线名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 是否停用
    /// </summary>
    public bool Disabled { get; private set; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 路线工序行列表
    /// </summary>
    public virtual ICollection<RoutingStep> RoutingSteps { get; private set; } = new List<RoutingStep>();

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Routing()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="code">路线代码</param>
    /// <param name="name">路线名称</param>
    /// <param name="remark">备注(可空)</param>
    internal Routing(string code, string name, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("路线代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("路线名称不能为空");
        }

        Code = code.Trim();
        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 获取路线工序行列表(只读)
    /// </summary>
    /// <returns>只读工序行列表</returns>
    public IReadOnlyList<RoutingStep> GetRoutingSteps()
    {
        return RoutingSteps.ToList().AsReadOnly();
    }

    /// <summary>
    /// 修改主字段(代码不可改)
    /// </summary>
    /// <param name="name">路线名称</param>
    /// <param name="remark">备注(可空)</param>
    public void Modify(string name, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("路线名称不能为空");
        }

        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 启用
    /// </summary>
    public void Enable()
    {
        Disabled = false;
    }

    /// <summary>
    /// 停用
    /// </summary>
    public void Disable()
    {
        Disabled = true;
    }

    /// <summary>
    /// 新增工序行
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="sequence">路线内顺序(相同值表示并行)</param>
    /// <param name="isMust">是否必须</param>
    /// <param name="timeInMins">工时定额(可空)</param>
    /// <param name="remark">备注(可空)</param>
    public void AddStep(long workStepId, int sequence, bool isMust, int? timeInMins, string? remark)
    {
        if (IsExistStep(workStepId))
        {
            throw new UserFriendlyException("工序已存在!");
        }

        RoutingSteps.Add(new RoutingStep(workStepId, sequence, isMust, timeInMins, remark));
    }

    /// <summary>
    /// 修改工序行
    /// </summary>
    /// <param name="routingStepId">工序行Id</param>
    /// <param name="workStepId">工序Id</param>
    /// <param name="sequence">路线内顺序</param>
    /// <param name="isMust">是否必须</param>
    /// <param name="timeInMins">工时定额(可空)</param>
    /// <param name="remark">备注(可空)</param>
    public void ModifyStep(long routingStepId, long workStepId, int sequence, bool isMust, int? timeInMins, string? remark)
    {
        RoutingStep? step = FindStep(routingStepId);
        if (step is null)
        {
            throw new UserFriendlyException("工序行不存在!");
        }

        if (step.WorkStepId != workStepId && IsExistStep(workStepId))
        {
            throw new UserFriendlyException("工序已存在!");
        }

        step.Modify(workStepId, sequence, isMust, timeInMins, remark);
    }

    /// <summary>
    /// 删除工序行
    /// </summary>
    /// <param name="routingStepId">工序行Id</param>
    public void DeleteStep(long routingStepId)
    {
        RoutingStep? step = FindStep(routingStepId);
        if (step is null)
        {
            return;
        }

        step.Delete();
    }

    /// <summary>
    /// 按路线内的顺序重排(从 1 连续编号, 并行工序保持同号)
    /// </summary>
    public void Resequence()
    {
        List<RoutingStep> sorted = RoutingSteps
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Sequence)
            .ToList();

        int sequence = 0;
        int? previous = null;
        foreach (RoutingStep step in sorted)
        {
            if (previous is null || previous.Value != step.Sequence)
            {
                sequence = sequence + 1;
                previous = step.Sequence;
            }

            step.ChangeSequence(sequence);
        }
    }

    /// <summary>
    /// 查找工序行
    /// </summary>
    /// <param name="routingStepId">工序行Id</param>
    /// <returns>工序行, 不存在返回 null</returns>
    private RoutingStep? FindStep(long routingStepId)
    {
        return RoutingSteps.FirstOrDefault(x => x.Id == routingStepId);
    }

    /// <summary>
    /// 工序是否已存在(未删除)
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistStep(long workStepId)
    {
        return RoutingSteps.Any(x => x.WorkStepId == workStepId && !x.IsDeleted);
    }
}
