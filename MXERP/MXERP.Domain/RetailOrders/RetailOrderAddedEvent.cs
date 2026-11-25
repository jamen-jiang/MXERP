using Snow.Domain.Events;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单新增事件
/// </summary>
public class RetailOrderAddedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="retailOrder">散镜订单</param>
    public RetailOrderAddedEvent(RetailOrder retailOrder)
        : base(retailOrder)
    {
    }
}
