using Snow.Domain.Events;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品保存事件(新增或修改)
/// </summary>
public class SalesSpuSavedEvent : DomainEvent
{
    /// <summary>
    /// 附件Id集合
    /// </summary>
    public IReadOnlyCollection<long>? AttachmentIds { get; }

    /// <summary>
    /// 打标附件Id集合
    /// </summary>
    public IReadOnlyCollection<long>? MarkingAttachmentIds { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="markingAttachmentIds">打标附件Id集合</param>
    public SalesSpuSavedEvent(SalesSpu salesSpu, IReadOnlyCollection<long>? attachmentIds, IReadOnlyCollection<long>? markingAttachmentIds)
        : base(salesSpu)
    {
        AttachmentIds = attachmentIds;
        MarkingAttachmentIds = markingAttachmentIds;
    }
}
