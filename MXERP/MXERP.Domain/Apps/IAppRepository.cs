using Snow.Domain.Repositories;

namespace MXERP.Domain.Apps;

/// <summary>
/// 应用仓储接口
/// </summary>
public interface IAppRepository : IRepository<App>
{
    /// <summary>
    /// 查询指定应用及其所有子应用(包含自身)
    /// </summary>
    /// <param name="id">应用Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>应用及其子应用列表</returns>
    Task<List<App>> GetWithChildrenAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询指定父级下的所有直接子应用
    /// </summary>
    /// <param name="parentId">父级Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>子应用实体列表</returns>
    Task<List<App>> GetByParentIdAsync(long parentId, CancellationToken cancellationToken = default);
}
