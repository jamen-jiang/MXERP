using Snow.Domain.Events;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品删除事件
/// </summary>
public class SalesSpuDeletedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpu">销售产品</param>
    public SalesSpuDeletedEvent(SalesSpu salesSpu)
        : base(salesSpu)
    {
    }
}
