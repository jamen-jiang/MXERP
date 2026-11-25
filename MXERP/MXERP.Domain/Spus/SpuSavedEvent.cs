using Snow.Domain.Events;

namespace MXERP.Domain.Spus;

/// <summary>
/// 产品保存事件(新增或修改)
/// </summary>
public class SpuSavedEvent : DomainEvent
{
    /// <summary>
    /// 公司Id集合
    /// </summary>
    public IReadOnlyCollection<long>? CompanyIds { get; }

    /// <summary>
    /// 附件Id集合
    /// </summary>
    public IReadOnlyCollection<long>? AttachmentIds { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spu">产品</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="attachmentIds">附件Id集合</param>
    public SpuSavedEvent(Spu spu, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds)
        : base(spu)
    {
        CompanyIds = companyIds;
        AttachmentIds = attachmentIds;
    }
}
