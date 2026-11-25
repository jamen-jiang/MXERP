using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.SpuCompanies;

/// <summary>
/// 产品公司关联
/// </summary>
public class SpuCompany : DeletionAuditedAggregateRoot
{
    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected SpuCompany()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="companyId">公司Id</param>
    public SpuCompany(long spuId, long companyId)
    {
        SpuId = spuId;
        CompanyId = companyId;
    }
}
