namespace MXERP.Domain.Roles;

public interface IRoleDomainService
{
    /// <summary>
    /// ModifyAsync(角色)
    /// </summary>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回角色</returns>
    Task<Role> ModifyAsync(long id, string name, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// ModifyAsync(角色)
    /// </summary>
    /// <param name="role">role</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Role role, string name, string? remark, CancellationToken cancellationToken = default);
}
