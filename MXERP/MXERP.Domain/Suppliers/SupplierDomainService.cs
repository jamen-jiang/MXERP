using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Suppliers;

/// <summary>
/// 供应商领域服务(对应 Java SupplierDomainService)
/// </summary>
[ScopedService]
public class SupplierDomainService : ISupplierDomainService
{
    private readonly SupplierFactory _factory;
    private readonly ISupplierRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">工厂</param>
    /// <param name="repository">仓储</param>
    public SupplierDomainService(SupplierFactory factory, ISupplierRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Supplier> ModifyAsync(
        long id,
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
        Supplier? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("供应商不存在!");
        }

        await ModifyAsync(model, code, oldCode, name, number, shortName, address, telephone, fax, email, contactPerson, taxRateId, payMode, deliveryMode, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(
        Supplier supplier,
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
        if (await _repository.IsExistByCodeExcludeIdAsync(code, supplier.Id, cancellationToken))
        {
            throw new UserFriendlyException($"供应商代码{code}已存在!");
        }

        supplier.Modify(code, oldCode, name, number, shortName, address, telephone, fax, email, contactPerson, taxRateId, payMode, deliveryMode, remark);
    }

    /// <inheritdoc />
    public async Task<Supplier> ImportGenerateAsync(
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
        Supplier? model = await _repository.GetByCodeAsync(code, cancellationToken);
        if (model is null)
        {
            return await _factory.CreateAsync(code, oldCode, name, number, shortName, address, telephone, fax, email, contactPerson, taxRateId, payMode, deliveryMode, remark, cancellationToken);
        }

        await ModifyAsync(model, code, oldCode, name, number, shortName, address, telephone, fax, email, contactPerson, taxRateId, payMode, deliveryMode, remark, cancellationToken);
        return model;
    }
}
