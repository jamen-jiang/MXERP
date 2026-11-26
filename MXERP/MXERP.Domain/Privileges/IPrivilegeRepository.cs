using Snow.Domain.Repositories;

namespace MXERP.Domain.Privileges;

/// <summary>
/// 权限仓储接口
/// </summary>
public interface IPrivilegeRepository : IRepository<Privilege>
{
    /// <summary>
    /// 按主体查询权限列表
    /// </summary>
    /// <param name="master">主体</param>
    /// <param name="masterValue">主体值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>权限列表</returns>
    Task<List<Privilege>> GetListByMasterAsync(string master, long masterValue, CancellationToken cancellationToken = default);
}
