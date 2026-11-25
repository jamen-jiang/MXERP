namespace MXERP.Domain.SpuCompanies;

/// <summary>
/// 产品公司关联领域服务接口
/// </summary>
public interface ISpuCompanyDomainService
{
    /// <summary>
    /// 生成产品公司关联的变更集合(新增 / 删除)
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="companyIds">公司Id集合(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>新增与删除的产品公司关联</returns>
    Task<SpuCompanyGenerateResult> CreateAsync(long spuId, IReadOnlyCollection<long>? companyIds, CancellationToken cancellationToken = default);
}
