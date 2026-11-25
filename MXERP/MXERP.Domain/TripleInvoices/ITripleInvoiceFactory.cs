namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票工厂接口
/// </summary>
public interface ITripleInvoiceFactory
{
    /// <summary>
    /// 创建三联发票
    /// </summary>
    Task<TripleInvoice> CreateAsync(TripleInvoiceData data, CancellationToken cancellationToken = default);
}
