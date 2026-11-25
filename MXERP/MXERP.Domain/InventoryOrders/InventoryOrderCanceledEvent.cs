using MXERP.Domain.Common.Aggregates;
using Snow.Domain.Events;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单取消事件
/// </summary>
public class InventoryOrderCanceledEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="inventoryOrder">盘点单</param>
    public InventoryOrderCanceledEvent(InventoryOrder inventoryOrder)
        : base(inventoryOrder)
    {
    }
}
