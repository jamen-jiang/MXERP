using Snow.Domain.Repositories;

namespace MXERP.Domain.SpuBrands;

/// <summary>
/// 产品品牌仓储接口
/// </summary>
public interface ISpuBrandRepository : IRepository<SpuBrand>
{
    /// <summary>
    /// 按品牌代码查询
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>品牌, 不存在返回 null</returns>
    Task<SpuBrand?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断品牌代码是否存在(排除指定主键)
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByCodeExcludeIdAsync(string code, long? id = null, CancellationToken cancellationToken = default);
}
