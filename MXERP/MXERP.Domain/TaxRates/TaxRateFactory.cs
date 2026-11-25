using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.TaxRates;

[ScopedService]
public class TaxRateFactory : ITaxRateFactory
{
    private readonly ITaxRateRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public TaxRateFactory(ITaxRateRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<TaxRate> CreateAsync(string code, string name, decimal rate, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"税率代码{code}已存在!");
        }

        return new TaxRate(code, name, rate, sort, remark);
    }
}
