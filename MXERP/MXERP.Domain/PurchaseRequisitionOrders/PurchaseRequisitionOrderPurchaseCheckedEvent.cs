using Snow.Domain.Events;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单采购审核事件
/// </summary>
public class PurchaseRequisitionOrderPurchaseCheckedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseRequisitionOrder">申购单</param>
    public PurchaseRequisitionOrderPurchaseCheckedEvent(PurchaseRequisitionOrder purchaseRequisitionOrder)
        : base(purchaseRequisitionOrder)
    {
    }
}
