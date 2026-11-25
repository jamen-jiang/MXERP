using Snow.Domain.Entities;
using Snow.Domain.Events;

namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 订单新增事件(用于生成流水号)
/// </summary>
public class OrderAddedEvent : DomainEvent
{
    /// <summary>
    /// 类型
    /// </summary>
    public SerialNumberTypeEnum Type { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="order">订单聚合根</param>
    /// <param name="type">流水号类型</param>
    public OrderAddedEvent(IAggregateRoot order, SerialNumberTypeEnum type)
        : base(order)
    {
        Type = type;
    }
}
