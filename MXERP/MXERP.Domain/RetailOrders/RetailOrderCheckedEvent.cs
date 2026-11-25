using Snow.Domain.Events;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单审核事件
/// </summary>
public class RetailOrderCheckedEvent : DomainEvent
{
    /// <summary>
    /// 散镜订单
    /// </summary>
    public RetailOrder RetailOrder { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="retailOrder">散镜订单</param>
    public RetailOrderCheckedEvent(RetailOrder retailOrder)
        : base(retailOrder)
    {
        RetailOrder = retailOrder;
    }
}
