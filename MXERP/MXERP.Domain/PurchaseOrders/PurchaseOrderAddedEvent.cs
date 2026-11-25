using MXERP.Domain.Common.Aggregates;
using Snow.Domain.Events;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单新增事件
/// </summary>
public class PurchaseOrderAddedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseOrder">采购单</param>
    public PurchaseOrderAddedEvent(PurchaseOrder purchaseOrder)
        : base(purchaseOrder)
    {
    }
}
