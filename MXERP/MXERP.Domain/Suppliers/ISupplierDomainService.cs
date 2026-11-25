namespace MXERP.Domain.Suppliers;

/// <summary>
/// 供应商领域服务接口(对应 Java SupplierDomainService)
/// </summary>
public interface ISupplierDomainService
{
    /// <summary>
    /// ModifyAsync(供应商)
    /// </summary>
    /// <returns>返回供应商</returns>
    Task<Supplier> ModifyAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(供应商)
    /// </summary>
    Task ModifyAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成供应商(存在则修改, 不存在则创建)
    /// </summary>
    /// <returns>返回供应商</returns>
    Task<Supplier> ImportGenerateAsync(
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
        CancellationToken cancellationToken = default);
}
