using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.CustomersSpus;

/// <summary>
/// 客人镜种仓储
/// </summary>
public interface ICustomerSpuRepository : IRepository<CustomerSpu>
{
    /// <summary>
    /// 按产品类型 + 代码查询
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>客人镜种，不存在返回 null</returns>
    Task<CustomerSpu?> GetByProductTypeAndCodeAsync(ProductTypeEnum productType, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断产品类型 + 代码是否存在
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="code">代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true：存在</returns>
    Task<bool> IsExistByProductTypeAndCodeAsync(ProductTypeEnum productType, string code, CancellationToken cancellationToken = default);
}
