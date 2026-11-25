using Snow.Domain.Repositories;

namespace MXERP.Domain.Users;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// 按账号查询用户
    /// </summary>
    /// <param name="userName">账号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户, 不存在返回 null</returns>
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按岗位Id列表批量查询用户
    /// </summary>
    /// <param name="postIds">岗位Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户列表</returns>
    Task<List<User>> GetListByPostIdsAsync(IReadOnlyCollection<long> postIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 名称是否已存在(排除指定主键)
    /// </summary>
    /// <param name="name">用户名称</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByNameExcludeIdAsync(string name, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 账号是否已存在(排除指定主键)
    /// </summary>
    /// <param name="userName">账号</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByUserNameExcludeIdAsync(string userName, long? id = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 邮箱是否已存在(排除指定主键)
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByEmailExcludeIdAsync(string email, long? id = null, CancellationToken cancellationToken = default);
}
