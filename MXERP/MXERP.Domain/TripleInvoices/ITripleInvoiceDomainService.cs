namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票领域服务接口
/// </summary>
public interface ITripleInvoiceDomainService
{
    /// <summary>
    /// 生成三联发票
    /// </summary>
    Task<TripleInvoice?> CreateAsync(long retailOrderId, int? customerInvoiceTitlePadNumber, CancellationToken cancellationToken = default);
}
