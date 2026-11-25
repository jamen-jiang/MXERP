using Snow.Domain.Events;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单取消事件
/// </summary>
public class LeaveStockOrderCanceledEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockOrder">出库单</param>
    public LeaveStockOrderCanceledEvent(LeaveStockOrder leaveStockOrder)
        : base(leaveStockOrder)
    {
        LeaveStockOrder = leaveStockOrder;
    }

    /// <summary>
    /// 出库单
    /// </summary>
    public LeaveStockOrder LeaveStockOrder { get; }
}
