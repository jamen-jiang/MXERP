using Snow.Domain.Repositories;

namespace MXERP.Domain.Warehouses;

public interface IWarehouseRepository : IRepository<Warehouse>
{
    /// <summary>
    /// 按代码获取仓库, 不存在返回 null
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>仓库, 不存在返回 null</returns>
    Task<Warehouse?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取默认仓库, 不存在返回 null
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>仓库, 不存在返回 null</returns>
    Task<Warehouse?> GetDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 代码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
