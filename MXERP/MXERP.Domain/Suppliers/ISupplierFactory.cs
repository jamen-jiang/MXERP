namespace MXERP.Domain.Suppliers;

/// <summary>
/// 供应商工厂接口(对应 Java SupplierFactory)
/// </summary>
public interface ISupplierFactory
{
    /// <summary>
    /// 创建供应商
    /// </summary>
    /// <returns>返回供应商</returns>
    Task<Supplier> CreateAsync(
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
