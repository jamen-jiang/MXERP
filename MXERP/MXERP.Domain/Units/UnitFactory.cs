using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Units;

[ScopedService]
public class UnitFactory : IUnitFactory
{
    private readonly IUnitRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public UnitFactory(IUnitRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Unit> CreateAsync(string code, string name, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"单位代码{code}已存在!");
        }

        return new Unit(code, name, sort, remark);
    }
}
