namespace MXERP.Domain.Users;

/// <summary>
/// 用户领域服务接口
/// </summary>
public interface IUserDomainService
{
    /// <summary>
    /// 修改用户(按Id查询后修改)
    /// </summary>
    /// <param name="id">用户Id</param>
    /// <param name="name">用户名称</param>
    /// <param name="userName">账号</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="password">密码</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="postIds">岗位Id列表</param>
    /// <param name="roleIds">角色Id列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回用户</returns>
    Task<User> ModifyAsync(long id, string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改用户(使用已有聚合)
    /// </summary>
    /// <param name="user">用户聚合</param>
    /// <param name="name">用户名称</param>
    /// <param name="userName">账号</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="password">密码</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="postIds">岗位Id列表</param>
    /// <param name="roleIds">角色Id列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(User user, string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds, CancellationToken cancellationToken = default);
}
