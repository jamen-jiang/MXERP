namespace MXERP.Domain.Roles;

public interface IRoleFactory
{
    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回角色</returns>
    Task<Role> CreateAsync(string name, string? remark, CancellationToken cancellationToken = default);
}
