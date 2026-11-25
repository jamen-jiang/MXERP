using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Warehouses;

[ScopedService]
public class WarehouseFactory : IWarehouseFactory
{
    private readonly IWarehouseRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public WarehouseFactory(IWarehouseRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Warehouse> CreateAsync(string code, string name, string? address, string? manageManName, string? telephone, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"仓库代码{code}已存在!");
        }

        return new Warehouse(code, name, address, manageManName, telephone, remark);
    }
}
