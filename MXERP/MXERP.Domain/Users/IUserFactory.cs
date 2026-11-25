namespace MXERP.Domain.Users;

/// <summary>
/// 用户工厂接口
/// </summary>
public interface IUserFactory
{
    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="name">用户名称</param>
    /// <param name="userName">账号</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="password">密码</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="postIds">岗位Id列表</param>
    /// <param name="roleIds">角色Id列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回用户</returns>
    Task<User> CreateAsync(string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds, CancellationToken cancellationToken = default);
}
