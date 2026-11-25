using MXERP.Domain.Common.Aggregates;
using Snow.Domain.Events;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单取消事件
/// </summary>
public class PurchaseOrderCanceledEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseOrder">采购单</param>
    public PurchaseOrderCanceledEvent(PurchaseOrder purchaseOrder)
        : base(purchaseOrder)
    {
    }
}
