using Snow.Domain.Events;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单创建信息修改事件
/// </summary>
public class LeaveWarehouseOrderCreateInfoModifiedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveWarehouseOrder">出仓单</param>
    public LeaveWarehouseOrderCreateInfoModifiedEvent(LeaveWarehouseOrder leaveWarehouseOrder) : base(leaveWarehouseOrder)
    {
        LeaveWarehouseOrder = leaveWarehouseOrder;
    }

    /// <summary>
    /// 出仓单
    /// </summary>
    public LeaveWarehouseOrder LeaveWarehouseOrder { get; }
}
