using Snow.Domain.Events;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单新增或修改事件
/// </summary>
public class ExternalLeaveStockOrderAddedOrModifiedEvent : DomainEvent
{
    /// <summary>
    /// 外部出库单
    /// </summary>
    public ExternalLeaveStockOrder ExternalLeaveStockOrder { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="externalLeaveStockOrder">外部出库单</param>
    public ExternalLeaveStockOrderAddedOrModifiedEvent(ExternalLeaveStockOrder externalLeaveStockOrder)
        : base(externalLeaveStockOrder)
    {
        ExternalLeaveStockOrder = externalLeaveStockOrder;
    }
}
