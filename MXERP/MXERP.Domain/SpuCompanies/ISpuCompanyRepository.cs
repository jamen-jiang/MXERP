using Snow.Domain.Repositories;

namespace MXERP.Domain.SpuCompanies;

/// <summary>
/// 产品公司关联仓储接口
/// </summary>
public interface ISpuCompanyRepository : IRepository<SpuCompany>
{
    /// <summary>
    /// 按产品Id查询
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品公司关联列表</returns>
    Task<List<SpuCompany>> GetBySpuIdAsync(long spuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按公司Id查询
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品公司关联列表</returns>
    Task<List<SpuCompany>> GetByCompanyIdAsync(long companyId, CancellationToken cancellationToken = default);
}
