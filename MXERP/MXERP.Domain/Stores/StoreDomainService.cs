using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Stores;

[ScopedService]
public class StoreDomainService : IStoreDomainService
{
    private readonly IStoreFactory _factory;
    private readonly IStoreRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">门店工厂</param>
    /// <param name="repository">仓储</param>
    public StoreDomainService(IStoreFactory factory, IStoreRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Store> ModifyAsync(long id, long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default)
    {
        Store? store = await _repository.FindAsync(id, true, cancellationToken);
        if (store is null)
        {
            throw new UserFriendlyException("门店不存在!");
        }

        await ModifyAsync(store, companyId, code, name, shortName, number, invoiceTitle, telephone, fax, email, cityCode, cityName, areaCode, postCode, logisticsCode, logisticsName, signer, convertCode, address, billingAddress, cancellationToken);
        return store;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Store store, long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, store.Id, cancellationToken))
        {
            throw new UserFriendlyException($"门店代码{code}已存在!");
        }

        store.Modify(companyId, code, name, shortName, number, invoiceTitle, telephone, fax, email, cityCode, cityName, areaCode, postCode, logisticsCode, logisticsName, signer, convertCode, address, billingAddress);
    }

    /// <inheritdoc />
    public async Task<Store> ImportGenerateAsync(long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress, CancellationToken cancellationToken = default)
    {
        Store? store = await _repository.GetByCodeAsync(code, cancellationToken);
        if (store is null)
        {
            store = await _factory.CreateAsync(companyId, code, name, shortName, number, invoiceTitle, telephone, fax, email, cityCode, cityName, areaCode, postCode, logisticsCode, logisticsName, signer, convertCode, address, billingAddress, cancellationToken);
        }
        else
        {
            await ModifyAsync(store, companyId, code, name, shortName, number, invoiceTitle, telephone, fax, email, cityCode, cityName, areaCode, postCode, logisticsCode, logisticsName, signer, convertCode, address, billingAddress, cancellationToken);
        }

        return store;
    }

    /// <inheritdoc />
    public Task SyncStoreContactsAsync(Store store, List<StoreContactData>? contacts, CancellationToken cancellationToken = default)
    {
        List<StoreContactData> target = contacts ?? new List<StoreContactData>();

        List<StoreContact> toRemoves = store.StoreContacts
            .Where(c => target.All(i => i.Name != c.Name))
            .ToList();
        foreach (StoreContact item in toRemoves)
        {
            store.DeleteContact(item.Id);
        }

        Dictionary<string, long> existingDict = new Dictionary<string, long>();
        foreach (StoreContact contact in store.StoreContacts)
        {
            if (!existingDict.ContainsKey(contact.Name))
            {
                existingDict[contact.Name] = contact.Id;
            }
        }

        foreach (StoreContactData item in target)
        {
            if (existingDict.TryGetValue(item.Name, out long id))
            {
                store.ModifyContact(id, item.CompanyId, item.Telephone, item.Area, item.Address);
            }
            else
            {
                store.AddContact(item.Name, item.CompanyId, item.Telephone, item.Area, item.Address);
            }
        }

        return Task.CompletedTask;
    }
}
