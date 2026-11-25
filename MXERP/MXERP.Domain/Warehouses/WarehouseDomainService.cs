using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Warehouses;

[ScopedService]
public class WarehouseDomainService : IWarehouseDomainService
{
    private readonly IWarehouseRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public WarehouseDomainService(IWarehouseRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<MarkDefaultResult> MarkDefaultAsync(long id, CancellationToken cancellationToken = default)
    {
        Warehouse? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("仓库不存在!");
        }

        Warehouse? currentDefault = await _repository.GetDefaultAsync(cancellationToken);
        Warehouse? oldDefault = null;
        if (currentDefault is not null && currentDefault.Id != model.Id)
        {
            currentDefault.UnmarkDefault();
            oldDefault = currentDefault;
        }

        model.MarkAsDefault();
        return new MarkDefaultResult(oldDefault, model);
    }
}
