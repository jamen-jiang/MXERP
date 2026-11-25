using Snow.Domain.Events;

namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票新增事件
/// </summary>
public class TripleInvoiceAddedEvent : DomainEvent
{
    /// <summary>
    /// 门店Id
    /// </summary>
    public long StoreId { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="tripleInvoice">三联发票</param>
    public TripleInvoiceAddedEvent(TripleInvoice tripleInvoice)
        : base(tripleInvoice)
    {
        StoreId = tripleInvoice.StoreId;
    }
}
