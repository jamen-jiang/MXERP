using Snow.Domain.Events;

namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单取消事件
/// </summary>
public class BulkOrderCanceledEvent : DomainEvent
{
    /// <summary>
    /// 批量订单
    /// </summary>
    public BulkOrder BulkOrder { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="bulkOrder">批量订单</param>
    public BulkOrderCanceledEvent(BulkOrder bulkOrder)
        : base(bulkOrder)
    {
        BulkOrder = bulkOrder;
    }
}
