using Snow.Domain.Events;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项确认事件
/// </summary>
public class BulkOrderItemConfirmedEvent : DomainEvent
{
    /// <summary>
    /// 批量订单Id
    /// </summary>
    public long BulkOrderId { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="bulkOrderItem">批量订单项</param>
    public BulkOrderItemConfirmedEvent(BulkOrderItem bulkOrderItem)
        : base(bulkOrderItem)
    {
        BulkOrderId = bulkOrderItem.BulkOrderId;
    }
}
