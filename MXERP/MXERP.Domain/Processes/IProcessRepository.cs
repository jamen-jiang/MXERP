using Snow.Domain.Repositories;

namespace MXERP.Domain.Processes;

/// <summary>
/// 工序仓储接口
/// </summary>
public interface IProcessRepository : IRepository<Process>
{
    /// <summary>
    /// 按类型和代码查询工序
    /// </summary>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序, 不存在返回 null</returns>
    Task<Process?> GetByTypeAndCodeAsync(string type, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断同类型下工序代码是否存在(排除指定主键)
    /// </summary>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByTypeAndCodeExcludeIdAsync(string type, string code, long? id = null, CancellationToken cancellationToken = default);
}
