using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Diameters;

[ScopedService]
public class DiameterDomainService : IDiameterDomainService
{
    private readonly DiameterFactory _factory;
    private readonly IDiameterRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">工厂</param>
    /// <param name="repository">仓储</param>
    public DiameterDomainService(DiameterFactory factory, IDiameterRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Diameter> ModifyAsync(long id, string color, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByColorExcludeIdAsync(color, id, cancellationToken))
        {
            throw new UserFriendlyException($"颜色{color}已存在!");
        }

        Diameter? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("直径不存在!");
        }

        model.Modify(color, remark);
        return model;
    }

    /// <inheritdoc />
    public async Task<MarkDefaultResult> MarkDefaultAsync(long id, CancellationToken cancellationToken = default)
    {
        Diameter? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("直径不存在!");
        }

        Diameter? currentDefault = await _repository.GetDefaultAsync(cancellationToken);
        Diameter? oldDefault = null;
        if (currentDefault is not null && currentDefault.Id != model.Id)
        {
            currentDefault.UnmarkDefault();
            oldDefault = currentDefault;
        }

        model.MarkAsDefault();
        return new MarkDefaultResult(oldDefault, model);
    }

    /// <inheritdoc />
    public async Task<Diameter> ImportGenerateAsync(int size, string color, string? remark, CancellationToken cancellationToken = default)
    {
        Diameter? model = await _repository.GetBySizeAsync(size, cancellationToken);
        if (model is null)
        {
            return await _factory.CreateAsync(size, color, remark, cancellationToken);
        }

        model.Modify(color, remark);
        return model;
    }
}
