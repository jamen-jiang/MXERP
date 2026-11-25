using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Routings;

/// <summary>
/// 路线工序行(相同 Sequence 表示并行工序)
/// </summary>
public class RoutingStep : DeletionAuditedEntity
{
    /// <summary>
    /// 路线Id
    /// </summary>
    public long? RoutingId { get; private set; }

    /// <summary>
    /// 路线内顺序(相同值 = 并行工序)
    /// </summary>
    public int Sequence { get; private set; }

    /// <summary>
    /// 工序Id
    /// </summary>
    public long WorkStepId { get; private set; }

    /// <summary>
    /// 是否必须
    /// </summary>
    public bool IsMust { get; private set; }

    /// <summary>
    /// 工时定额(可空)
    /// </summary>
    public int? TimeInMins { get; private set; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected RoutingStep()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="sequence">路线内顺序</param>
    /// <param name="isMust">是否必须</param>
    /// <param name="timeInMins">工时定额(可空)</param>
    /// <param name="remark">备注(可空)</param>
    internal RoutingStep(long workStepId, int sequence, bool isMust, int? timeInMins, string? remark)
    {
        if (workStepId <= 0)
        {
            throw new UserFriendlyException("工序不能为空");
        }

        WorkStepId = workStepId;
        Sequence = sequence;
        IsMust = isMust;
        TimeInMins = timeInMins;
        Remark = remark;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="sequence">路线内顺序</param>
    /// <param name="isMust">是否必须</param>
    /// <param name="timeInMins">工时定额(可空)</param>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(long workStepId, int sequence, bool isMust, int? timeInMins, string? remark)
    {
        if (workStepId <= 0)
        {
            throw new UserFriendlyException("工序不能为空");
        }

        WorkStepId = workStepId;
        Sequence = sequence;
        IsMust = isMust;
        TimeInMins = timeInMins;
        Remark = remark;
    }

    /// <summary>
    /// 变更顺序(供父聚合重排使用)
    /// </summary>
    /// <param name="sequence">路线内顺序</param>
    internal void ChangeSequence(int sequence)
    {
        Sequence = sequence;
    }
}
