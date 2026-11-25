using Snow.Domain.Events;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单完成事件
/// </summary>
public class LeaveStockOrderFinishedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockOrder">出库单</param>
    public LeaveStockOrderFinishedEvent(LeaveStockOrder leaveStockOrder)
        : base(leaveStockOrder)
    {
        LeaveStockOrder = leaveStockOrder;
    }

    /// <summary>
    /// 出库单
    /// </summary>
    public LeaveStockOrder LeaveStockOrder { get; }
}
