using Snow.Domain.Events;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单完成事件
/// </summary>
public class EnterStockOrderFinishedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="enterStockOrder">入库单</param>
    public EnterStockOrderFinishedEvent(EnterStockOrder enterStockOrder)
        : base(enterStockOrder)
    {
        EnterStockOrder = enterStockOrder;
    }

    /// <summary>
    /// 入库单
    /// </summary>
    public EnterStockOrder EnterStockOrder { get; }
}
