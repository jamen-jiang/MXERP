using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品仓储接口
/// </summary>
public interface ISalesSpuRepository : IRepository<SalesSpu>
{
    /// <summary>
    /// 按产品类型 + 公司Id + 代码查询
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品, 不存在返回 null</returns>
    Task<SalesSpu?> GetByProductTypeAndCompanyIdAndCodeAsync(ProductTypeEnum productType, long companyId, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断产品类型 + 公司Id + 代码是否存在(排除指定主键)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByProductTypeAndCompanyIdAndCodeExcludeIdAsync(ProductTypeEnum productType, long companyId, string code, long? id = null, CancellationToken cancellationToken = default);
}
