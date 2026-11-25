using Snow.Domain.Repositories;

namespace MXERP.Domain.Ranks;

/// <summary>
/// 职级仓储(对应 Java RankRepository)
/// </summary>
public interface IRankRepository : IRepository<Rank>
{
    /// <summary>
    /// 获取职级及其所有子级
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>职级列表</returns>
    Task<List<Rank>> GetWithChildrenAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取指定上级下的职级列表
    /// </summary>
    /// <param name="parentId">上级Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>职级列表</returns>
    Task<List<Rank>> GetByParentIdAsync(long parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 是否已存在(排除指定主键)
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="parentId">上级Id</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistAsync(string name, long? parentId, long? id = null, CancellationToken cancellationToken = default);
}
