using Snow.Domain.Repositories;

namespace MXERP.Domain.Posts;

/// <summary>
/// 岗位仓储接口
/// </summary>
public interface IPostRepository : IRepository<Post>
{
    /// <summary>
    /// 按岗位名称查询
    /// </summary>
    /// <param name="name">岗位名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>岗位, 不存在返回 null</returns>
    Task<Post?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按部门Id列表批量查询
    /// </summary>
    /// <param name="departmentIds">部门Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>岗位列表</returns>
    Task<List<Post>> GetListByDepartmentIdsAsync(IReadOnlyCollection<long> departmentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 岗位名称是否已存在(排除指定主键)
    /// </summary>
    /// <param name="name">岗位名称</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default);
}
