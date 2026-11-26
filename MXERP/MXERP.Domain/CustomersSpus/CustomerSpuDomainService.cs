using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;

namespace MXERP.Domain.CustomersSpus;

/// <summary>
/// 客人镜种领域服务
/// </summary>
[ScopedService]
public class CustomerSpuDomainService : ICustomerSpuDomainService
{
    private readonly CustomerSpuFactory _customerSpuFactory;
    private readonly ICustomerSpuRepository _customerSpuRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="customerSpuFactory">客人镜种工厂</param>
    /// <param name="customerSpuRepository">客人镜种仓储</param>
    public CustomerSpuDomainService(CustomerSpuFactory customerSpuFactory, ICustomerSpuRepository customerSpuRepository)
    {
        _customerSpuFactory = customerSpuFactory;
        _customerSpuRepository = customerSpuRepository;
    }

    /// <inheritdoc />
    public async Task<CustomerSpu> ImportGenerateAsync(string productType, string code, string name, string? bagNumber, long salesSpuId, long brandId, CancellationToken cancellationToken = default)
    {
        CustomerSpu? model = await _customerSpuRepository.GetByProductTypeAndCodeAsync(productType, code, cancellationToken);
        if (model == null)
        {
            model = await _customerSpuFactory.CreateAsync(productType, code, name, bagNumber, salesSpuId, brandId, null, cancellationToken);
        }
        else
        {
            model.Modify(name, bagNumber, salesSpuId, brandId);
        }

        return model;
    }
}
