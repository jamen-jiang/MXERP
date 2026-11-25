using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Positions;

[ScopedService]
public class PositionFactory : IPositionFactory
{
    private readonly IPositionRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public PositionFactory(IPositionRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Position> CreateAsync(string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"仓位代码{code}已存在!");
        }

        return new Position(category, code, name, shortName, endProductCode, semiProductCode, sort, remark);
    }
}
