using Snow.Domain.Events;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单取消事件
/// </summary>
public class EnterStockOrderCanceledEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="enterStockOrder">入库单</param>
    public EnterStockOrderCanceledEvent(EnterStockOrder enterStockOrder)
        : base(enterStockOrder)
    {
        EnterStockOrder = enterStockOrder;
    }

    /// <summary>
    /// 入库单
    /// </summary>
    public EnterStockOrder EnterStockOrder { get; }
}
