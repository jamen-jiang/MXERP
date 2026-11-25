using Snow.DependencyInjection;

namespace MXERP.Domain.Currencies;

/// <summary>
/// 币种领域服务: 存在则修改, 不存在则创建
/// </summary>
[ScopedService]
public class CurrencyDomainService : ICurrencyDomainService
{
    private readonly CurrencyFactory _factory;
    private readonly ICurrencyRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">工厂</param>
    /// <param name="repository">仓储</param>
    public CurrencyDomainService(CurrencyFactory factory, ICurrencyRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Currency> ImportGenerateAsync(string code, string name, string symbol, decimal acceptRate, decimal adjustRate, bool isStandard, bool isEnable, CancellationToken cancellationToken = default)
    {
        Currency? model = await _repository.GetByCodeAsync(code, cancellationToken);
        if (model is null)
        {
            return await _factory.CreateAsync(code, name, symbol, acceptRate, adjustRate, isStandard, isEnable, cancellationToken);
        }

        model.Modify(name, symbol, acceptRate, adjustRate, isStandard, isEnable);
        return model;
    }
}
