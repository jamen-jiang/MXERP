using Snow.Domain.Events;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次取消事件
/// </summary>
public class PurchaseBatchCanceledEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseBatch">采购批次</param>
    public PurchaseBatchCanceledEvent(PurchaseBatch purchaseBatch)
        : base(purchaseBatch)
    {
        PurchaseBatch = purchaseBatch;
    }

    /// <summary>
    /// 采购批次
    /// </summary>
    public PurchaseBatch PurchaseBatch { get; }
}
