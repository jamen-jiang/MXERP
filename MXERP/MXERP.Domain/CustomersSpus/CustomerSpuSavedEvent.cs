using Snow.Domain.Entities.Auditing;
using Snow.Domain.Events;

namespace MXERP.Domain.CustomersSpus;

/// <summary>
/// 客人镜种保存事件
/// </summary>
public class CustomerSpuSavedEvent : DomainEvent
{
    /// <summary>
    /// 附件Id（可空）
    /// </summary>
    public IReadOnlyCollection<long>? AttachmentIds { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="customerSpu">客人镜种</param>
    /// <param name="attachmentIds">附件Id（可空）</param>
    public CustomerSpuSavedEvent(CustomerSpu customerSpu, IReadOnlyCollection<long>? attachmentIds)
        : base(customerSpu)
    {
        AttachmentIds = attachmentIds;
    }
}
