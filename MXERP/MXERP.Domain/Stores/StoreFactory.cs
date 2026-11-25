using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Stores;

[ScopedService]
public class StoreFactory : IStoreFactory
{
    private readonly IStoreRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public StoreFactory(IStoreRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Store> CreateAsync(long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"门店代码{code}已存在!");
        }

        return new Store(companyId, code, name, shortName, number, invoiceTitle, telephone, fax, email, cityCode, cityName, areaCode, postCode, logisticsCode, logisticsName, signer, convertCode, address, billingAddress);
    }
}
