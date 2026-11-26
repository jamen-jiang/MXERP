using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Companies;

[ScopedService]
public class CompanyFactory : ICompanyFactory
{
    private readonly ICompanyRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public CompanyFactory(ICompanyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Company> CreateAsync(string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"公司代码{code}已存在!");
        }

        return new Company(region, group, code, name, shortName, numberCode, telephone, fax, email, areaCode, postCode, currencyId, address, billingAddress, sort, remark);
    }
}
