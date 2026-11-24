using Snow.Domain.Repositories;

namespace MXERP.Domain.Brands;

/// <summary>品牌仓储(对应 Java 的 BrandRepository, 额外暴露按代码查询/判重)</summary>
public interface IBrandRepository : IRepository<Brand>
{
    /// <summary>按代码取品牌(未删除的), 取不到返回 null</summary>
    /// <param name="code">品牌代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>品牌或 null</returns>
    Task<Brand?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>代码是否已存在(未删除的)</summary>
    /// <param name="code">品牌代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>存在返回 true</returns>
    Task<bool> IsExistByCodeAsync(string code, CancellationToken cancellationToken = default);
}
