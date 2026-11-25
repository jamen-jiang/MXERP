using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Currencies;

/// <summary>
/// 币种工厂: 创建前校验代码唯一
/// </summary>
[ScopedService]
public class CurrencyFactory : ICurrencyFactory
{
    private readonly ICurrencyRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public CurrencyFactory(ICurrencyRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Currency> CreateAsync(string code, string name, string symbol, decimal acceptRate, decimal adjustRate, bool isStandard, bool isEnable, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"币种代码{code}已存在!");
        }

        return new Currency(code, name, symbol, acceptRate, adjustRate, isStandard, isEnable);
    }
}
