using Snow.Domain.Events;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项新增或修改事件
/// </summary>
public class BulkOrderItemSavedEvent : DomainEvent
{
    /// <summary>
    /// 附件Id集合(可空)
    /// </summary>
    public IReadOnlyCollection<long>? AttachmentIds { get; }
    /// <summary>
    /// 附件集合(可空)
    /// </summary>
    public IReadOnlyDictionary<string, byte[]>? Attachments { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="attachmentIds">附件Id集合(可空)</param>
    /// <param name="attachments">附件集合(可空)</param>
    public BulkOrderItemSavedEvent(BulkOrderItem bulkOrderItem, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments)
        : base(bulkOrderItem)
    {
        AttachmentIds = attachmentIds;
        Attachments = attachments;
    }
}
