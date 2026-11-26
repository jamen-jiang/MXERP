using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.CustomersSpus;

/// <summary>
/// 客人镜种工厂
/// </summary>
[ScopedService]
public class CustomerSpuFactory : ICustomerSpuFactory
{
    private readonly ICustomerSpuRepository _customerSpuRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="customerSpuRepository">客人镜种仓储</param>
    public CustomerSpuFactory(ICustomerSpuRepository customerSpuRepository)
    {
        _customerSpuRepository = customerSpuRepository;
    }

    /// <inheritdoc />
    public async Task<CustomerSpu> CreateAsync(string productType, string code, string name, string? bagNumber, long salesSpuId, long brandId, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default)
    {
        if (await _customerSpuRepository.IsExistByProductTypeAndCodeAsync(productType, code, cancellationToken))
        {
            throw new UserFriendlyException("客人产品" + code + "已存在!");
        }

        return new CustomerSpu(productType, salesSpuId, brandId, code, name, bagNumber, attachmentIds);
    }
}
