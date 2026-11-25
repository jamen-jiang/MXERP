namespace MXERP.Domain.Skus;

/// <summary>
/// SKU领域服务接口
/// </summary>
public interface ISkuDomainService
{
    /// <summary>
    /// 创建SKU(入参为Id, 不解析Code)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="spuId">SpuId</param>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU聚合</returns>
    Task<Sku> CreateAsync(string productType, long spuId, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成(存在则修改, 不存在则新建)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU聚合</returns>
    Task<Sku> ImportGenerateAsync(string productType, long spuId, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改(按Id查询后修改)
    /// </summary>
    /// <param name="id">SKU Id</param>
    /// <param name="opc">OPC</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="attributes">属性值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU聚合</returns>
    Task<Sku> ModifyAsync(long id, string? opc, decimal? purchasePrice, long? currencyId, long? unitId, long? taxRateId, decimal? moq, SortedDictionary<string, object?>? attributes, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从Spu生成SKU(按产品类型分流)
    /// </summary>
    /// <param name="spuId">SpuId</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>需要持久化的聚合列表(含新增 + 已标记删除的)</returns>
    Task<List<Sku>> CreateFromSpuAsync(long spuId, CancellationToken cancellationToken = default);
}
