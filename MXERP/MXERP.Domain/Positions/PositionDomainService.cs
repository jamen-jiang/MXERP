using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Positions;

[ScopedService]
public class PositionDomainService : IPositionDomainService
{
    private readonly PositionFactory _factory;
    private readonly IPositionRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">工厂</param>
    /// <param name="repository">仓储</param>
    public PositionDomainService(PositionFactory factory, IPositionRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Position> ModifyAsync(long id, string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        Position? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("仓位不存在!");
        }

        await ModifyAsync(model, category, code, name, shortName, endProductCode, semiProductCode, sort, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Position position, string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, position.Id, cancellationToken))
        {
            throw new UserFriendlyException($"仓位代码{code}已存在!");
        }

        position.Modify(category, code, name, shortName, endProductCode, semiProductCode, sort, remark);
    }
}
