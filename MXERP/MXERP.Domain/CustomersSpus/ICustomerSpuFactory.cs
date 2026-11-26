using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.CustomersSpus;

/// <summary>
/// 客人镜种工厂
/// </summary>
public interface ICustomerSpuFactory
{
    /// <summary>
    /// 创建客人镜种
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="bagNumber">镜袋编号（可空）</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="attachmentIds">附件Id集合（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>客人镜种</returns>
    Task<CustomerSpu> CreateAsync(string productType, string code, string name, string? bagNumber, long salesSpuId, long brandId, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default);
}
