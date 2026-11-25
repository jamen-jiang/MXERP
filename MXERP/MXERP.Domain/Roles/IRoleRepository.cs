using Snow.Domain.Repositories;

namespace MXERP.Domain.Roles;

public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// 按名称获取角色, 不存在返回 null
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色, 不存在返回 null</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// 名称是否已存在(排除指定主键)
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default);
}
