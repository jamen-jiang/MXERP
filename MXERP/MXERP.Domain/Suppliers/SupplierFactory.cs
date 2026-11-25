using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Suppliers;

/// <summary>
/// 供应商工厂(对应 Java SupplierFactory): 创建前校验代码唯一
/// </summary>
[ScopedService]
public class SupplierFactory : ISupplierFactory
{
    private readonly ISupplierRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public SupplierFactory(ISupplierRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Supplier> CreateAsync(
        string code,
        string? oldCode,
        string name,
        string? number,
        string? shortName,
        string? address,
        string? telephone,
        string? fax,
        string? email,
        string? contactPerson,
        long? taxRateId,
        string? payMode,
        string? deliveryMode,
        string? remark,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"供应商代码{code}已存在!");
        }

        return new Supplier(code, oldCode, name, number, shortName, address, telephone, fax, email, contactPerson, taxRateId, payMode, deliveryMode, remark);
    }
}
