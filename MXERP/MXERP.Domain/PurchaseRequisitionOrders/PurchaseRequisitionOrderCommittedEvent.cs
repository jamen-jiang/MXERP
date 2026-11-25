using Snow.Domain.Events;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单提交事件
/// </summary>
public class PurchaseRequisitionOrderCommittedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseRequisitionOrder">申购单</param>
    public PurchaseRequisitionOrderCommittedEvent(PurchaseRequisitionOrder purchaseRequisitionOrder)
        : base(purchaseRequisitionOrder)
    {
        Number = purchaseRequisitionOrder.Number;
        DepartmentId = purchaseRequisitionOrder.DepartmentId;
        CreatorId = purchaseRequisitionOrder.CreatorId;
    }

    /// <summary>
    /// 单号
    /// </summary>
    public string Number { get; }
    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; }
    /// <summary>
    /// 创建人Id（可空）
    /// </summary>
    public long? CreatorId { get; }
}
