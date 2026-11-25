using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.SalesSkus;

/// <summary>
/// 销售SKU领域服务接口
/// </summary>
public interface ISalesSkuDomainService
{
    /// <summary>
    /// 创建销售SKU(入参为Id, 不解析Code)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="opc">Opc</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售SKU聚合</returns>
    Task<SalesSku> CreateAsync(ProductTypeEnum productType, long salesSpuId, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成(存在则修改, 不存在则新建)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="opc">Opc</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售SKU聚合</returns>
    Task<SalesSku> ImportGenerateAsync(ProductTypeEnum productType, long salesSpuId, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改(按Id查询后修改)
    /// </summary>
    /// <param name="id">销售SKU Id</param>
    /// <param name="opc">Opc</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售SKU聚合</returns>
    Task<SalesSku> ModifyAsync(long id, string? opc, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从销售产品生成销售SKU(按产品类型分流)
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>需要持久化的聚合列表(含新增 + 已标记删除的)</returns>
    Task<List<SalesSku>> CreateFromSalesSpuAsync(long salesSpuId, CancellationToken cancellationToken = default);
}
