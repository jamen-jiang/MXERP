using Snow.Domain.Repositories;

namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票仓储接口
/// </summary>
public interface ITripleInvoiceRepository : IRepository<TripleInvoice>
{
    /// <summary>
    /// 按单号查询
    /// </summary>
    Task<TripleInvoice?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按客户发票抬头查询单号最大的三联发票
    /// </summary>
    Task<TripleInvoice?> GetMaxByStoreInvoiceTitleAsync(string storeInvoiceTitle, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断散镜订单Id是否存在
    /// </summary>
    Task<bool> IsExistByRetailOrderIdAsync(long retailOrderId, CancellationToken cancellationToken = default);
}
