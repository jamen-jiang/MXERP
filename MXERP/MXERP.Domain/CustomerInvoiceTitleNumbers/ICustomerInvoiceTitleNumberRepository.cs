using Snow.Domain.Repositories;

namespace MXERP.Domain.CustomerInvoiceTitleNumbers;

/// <summary>
/// 客户票号仓储接口
/// </summary>
public interface ICustomerInvoiceTitleNumberRepository : IRepository<CustomerInvoiceTitleNumber>
{
    /// <summary>
    /// 按发票抬头查询客户票号
    /// </summary>
    /// <param name="invoiceTitle">发票抬头</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>客户票号, 不存在返回 null</returns>
    Task<CustomerInvoiceTitleNumber?> GetByInvoiceTitleAsync(string invoiceTitle, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断发票抬头是否存在(排除指定主键)
    /// </summary>
    /// <param name="invoiceTitle">发票抬头</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByInvoiceTitleExcludeIdAsync(string invoiceTitle, long? id = null, CancellationToken cancellationToken = default);
}
