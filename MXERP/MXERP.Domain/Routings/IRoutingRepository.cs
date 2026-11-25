using Snow.Domain.Repositories;

namespace MXERP.Domain.Routings;

/// <summary>
/// 工艺路线仓储接口
/// </summary>
public interface IRoutingRepository : IRepository<Routing>
{
    /// <summary>
    /// 按代码查询
    /// </summary>
    /// <param name="code">路线代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路线, 不存在返回 null</returns>
    Task<Routing?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询全部路线
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>路线列表</returns>
    Task<List<Routing>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断路线代码是否存在(排除指定主键)
    /// </summary>
    /// <param name="code">路线代码</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
