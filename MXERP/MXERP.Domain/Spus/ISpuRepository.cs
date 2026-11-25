using Snow.Domain.Repositories;

namespace MXERP.Domain.Spus;

/// <summary>
/// 产品仓储接口
/// </summary>
public interface ISpuRepository : IRepository<Spu>
{
    /// <summary>
    /// 按Id集合批量查询
    /// </summary>
    /// <param name="ids">主键集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品列表</returns>
    Task<List<Spu>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按产品类型 + 产品代码查询
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="code">产品代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品, 不存在返回 null</returns>
    Task<Spu?> GetByProductTypeAndCodeAsync(string productType, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断产品代码是否存在(排除指定主键)
    /// </summary>
    /// <param name="code">产品代码</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
