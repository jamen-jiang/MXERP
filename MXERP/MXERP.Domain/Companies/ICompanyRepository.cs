using Snow.Domain.Repositories;

namespace MXERP.Domain.Companies;

/// <summary>
/// 公司仓储接口
/// </summary>
public interface ICompanyRepository : IRepository<Company>
{
    /// <summary>
    /// 按代码查询公司
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公司, 不存在返回 null</returns>
    Task<Company?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按Id列表查询公司
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公司列表</returns>
    Task<List<Company>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 公司代码是否已存在(排除指定主键)
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="id">主键, 为空表示不排除</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
