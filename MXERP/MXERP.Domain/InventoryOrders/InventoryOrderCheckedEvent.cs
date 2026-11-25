using MXERP.Domain.Common.Aggregates;
using Snow.Domain.Events;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单审核事件
/// </summary>
public class InventoryOrderCheckedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="inventoryOrder">盘点单</param>
    public InventoryOrderCheckedEvent(InventoryOrder inventoryOrder)
        : base(inventoryOrder)
    {
    }
}
