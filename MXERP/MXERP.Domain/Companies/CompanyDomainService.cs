using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Companies;

[ScopedService]
public class CompanyDomainService : ICompanyDomainService
{
    private readonly ICompanyFactory _factory;
    private readonly ICompanyRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">公司工厂</param>
    /// <param name="repository">仓储</param>
    public CompanyDomainService(ICompanyFactory factory, ICompanyRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Company> ModifyAsync(long id, CompanyRegionEnum region, CompanyGroupEnum? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        Company? company = await _repository.FindAsync(id, true, cancellationToken);
        if (company is null)
        {
            throw new UserFriendlyException("公司不存在!");
        }

        await ModifyAsync(company, region, group, code, name, shortName, numberCode, telephone, fax, email, areaCode, postCode, currencyId, address, billingAddress, sort, remark, cancellationToken);
        return company;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Company company, CompanyRegionEnum region, CompanyGroupEnum? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, company.Id, cancellationToken))
        {
            throw new UserFriendlyException($"公司代码{code}已存在!");
        }

        company.Modify(region, group, code, name, shortName, numberCode, telephone, fax, email, areaCode, postCode, currencyId, address, billingAddress, sort, remark);
    }

    /// <inheritdoc />
    public async Task<Company> ImportGenerateAsync(CompanyRegionEnum region, CompanyGroupEnum? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        Company? company = await _repository.GetByCodeAsync(code, cancellationToken);
        if (company is null)
        {
            company = await _factory.CreateAsync(region, group, code, name, shortName, numberCode, telephone, fax, email, areaCode, postCode, currencyId, address, billingAddress, sort, remark, cancellationToken);
        }
        else
        {
            await ModifyAsync(company, region, group, code, name, shortName, numberCode, telephone, fax, email, areaCode, postCode, currencyId, address, billingAddress, sort, remark, cancellationToken);
        }

        return company;
    }

    /// <inheritdoc />
    public Task SyncCompanyContactsAsync(Company company, List<CompanyContactData>? contacts, CancellationToken cancellationToken = default)
    {
        List<CompanyContactData> target = contacts ?? new List<CompanyContactData>();

        List<CompanyContact> toRemoves = company.CompanyContacts
            .Where(c => target.All(i => i.Name != c.Name))
            .ToList();
        foreach (CompanyContact item in toRemoves)
        {
            company.DeleteContact(item.Id);
        }

        Dictionary<string, long> existingDict = new Dictionary<string, long>();
        foreach (CompanyContact contact in company.CompanyContacts)
        {
            if (!existingDict.ContainsKey(contact.Name))
            {
                existingDict[contact.Name] = contact.Id;
            }
        }

        foreach (CompanyContactData item in target)
        {
            if (existingDict.TryGetValue(item.Name, out long id))
            {
                company.ModifyContact(id, item.Telephone, item.Area, item.Address);
            }
            else
            {
                company.AddContact(item.Name, item.Telephone, item.Area, item.Address);
            }
        }

        return Task.CompletedTask;
    }
}
