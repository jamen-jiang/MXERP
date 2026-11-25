using Snow.Domain.Repositories;

namespace MXERP.Domain.Suppliers;

/// <summary>
/// 供应商仓储(对应 Java SupplierRepository)
/// </summary>
public interface ISupplierRepository : IRepository<Supplier>
{
    /// <summary>
    /// 按代码获取供应商, 不存在返回 null
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>供应商, 不存在返回 null</returns>
    Task<Supplier?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 代码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
