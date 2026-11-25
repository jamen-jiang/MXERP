using Snow.DependencyInjection;

namespace MXERP.Domain.SpuCompanies;

[ScopedService]
public class SpuCompanyDomainService : ISpuCompanyDomainService
{
    private readonly ISpuCompanyRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">产品公司关联仓储</param>
    public SpuCompanyDomainService(ISpuCompanyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<SpuCompanyGenerateResult> CreateAsync(long spuId, IReadOnlyCollection<long>? companyIds, CancellationToken cancellationToken = default)
    {
        List<SpuCompany> removedSpuCompanies = await _repository.GetBySpuIdAsync(spuId, cancellationToken);
        foreach (SpuCompany spuCompany in removedSpuCompanies)
        {
            spuCompany.Delete();
        }

        List<SpuCompany> addedSpuCompanies = new List<SpuCompany>();
        if (companyIds is not null && companyIds.Count > 0)
        {
            foreach (long companyId in companyIds.Distinct())
            {
                addedSpuCompanies.Add(new SpuCompany(spuId, companyId));
            }
        }

        return new SpuCompanyGenerateResult(addedSpuCompanies, removedSpuCompanies);
    }
}
