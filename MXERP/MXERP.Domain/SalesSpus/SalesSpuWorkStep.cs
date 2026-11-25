using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品工序
/// </summary>
public class SalesSpuWorkStep : DeletionAuditedEntity
{
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long? SalesSpuId { get; private set; }

    /// <summary>
    /// 工序Id
    /// </summary>
    public long WorkStepId { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected SalesSpuWorkStep()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    internal SalesSpuWorkStep(long workStepId)
    {
        WorkStepId = workStepId;
    }
}
