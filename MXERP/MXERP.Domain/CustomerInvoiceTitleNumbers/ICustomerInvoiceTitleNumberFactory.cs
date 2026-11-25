namespace MXERP.Domain.CustomerInvoiceTitleNumbers;

/// <summary>
/// 客户票号工厂接口
/// </summary>
public interface ICustomerInvoiceTitleNumberFactory
{
    /// <summary>
    /// 创建客户票号
    /// </summary>
    /// <param name="invoiceTitle">发票抬头</param>
    /// <param name="padNumber">补充数字</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>客户票号聚合</returns>
    Task<CustomerInvoiceTitleNumber> CreateAsync(string invoiceTitle, int padNumber, CancellationToken cancellationToken = default);
}
