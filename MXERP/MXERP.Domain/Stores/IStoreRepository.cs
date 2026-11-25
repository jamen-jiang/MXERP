using Snow.Domain.Repositories;

namespace MXERP.Domain.Stores;

/// <summary>
/// 门店仓储接口
/// </summary>
public interface IStoreRepository : IRepository<Store>
{
    /// <summary>
    /// 按门店代码查询
    /// </summary>
    /// <param name="code">门店代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>门店, 不存在返回 null</returns>
    Task<Store?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按Id列表查询门店
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>门店列表</returns>
    Task<List<Store>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按门店代码列表批量查询
    /// </summary>
    /// <param name="codes">门店代码列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>门店列表</returns>
    Task<List<Store>> GetByListCodesAsync(IReadOnlyCollection<string> codes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 门店代码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="code">门店代码</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
