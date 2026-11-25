using Snow.Domain.Events;

namespace MXERP.Domain.Spus;

/// <summary>
/// 产品删除事件
/// </summary>
public class SpuDeletedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spu">产品</param>
    public SpuDeletedEvent(Spu spu)
        : base(spu)
    {
    }
}
