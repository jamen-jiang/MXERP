using Snow.Domain.Events;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单完成事件
/// </summary>
public class RetailOrderFinishedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="retailOrder">散镜订单</param>
    public RetailOrderFinishedEvent(RetailOrder retailOrder)
        : base(retailOrder)
    {
    }
}
