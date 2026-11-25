using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.CustomerInvoiceTitleNumbers;

[ScopedService]
public class CustomerInvoiceTitleNumberFactory : ICustomerInvoiceTitleNumberFactory
{
    private readonly ICustomerInvoiceTitleNumberRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">客户票号仓储</param>
    public CustomerInvoiceTitleNumberFactory(ICustomerInvoiceTitleNumberRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<CustomerInvoiceTitleNumber> CreateAsync(string invoiceTitle, int padNumber, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByInvoiceTitleExcludeIdAsync(invoiceTitle, null, cancellationToken))
        {
            throw new UserFriendlyException($"发票抬头{invoiceTitle}已存在!");
        }

        return new CustomerInvoiceTitleNumber(invoiceTitle, padNumber);
    }
}
