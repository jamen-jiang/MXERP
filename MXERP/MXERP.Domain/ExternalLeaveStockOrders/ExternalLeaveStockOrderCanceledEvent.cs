using Snow.Domain.Events;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单取消事件
/// </summary>
public class ExternalLeaveStockOrderCanceledEvent : DomainEvent
{
    /// <summary>
    /// 外部出库单
    /// </summary>
    public ExternalLeaveStockOrder ExternalLeaveStockOrder { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="externalLeaveStockOrder">外部出库单</param>
    public ExternalLeaveStockOrderCanceledEvent(ExternalLeaveStockOrder externalLeaveStockOrder)
        : base(externalLeaveStockOrder)
    {
        ExternalLeaveStockOrder = externalLeaveStockOrder;
    }
}
