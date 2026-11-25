namespace MXERP.Domain.Spus;

/// <summary>
/// 产品领域服务接口
/// </summary>
public interface ISpuDomainService
{
    /// <summary>
    /// 创建产品
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="group">组别</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">产品代码</param>
    /// <param name="name">产品名称</param>
    /// <param name="description">产品描述</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="remark">备注</param>
    /// <param name="attributes">属性值</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品聚合</returns>
    Task<Spu> CreateAsync(string productType, string? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成产品(存在则修改, 不存在则新建)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="group">组别</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">产品代码</param>
    /// <param name="name">产品名称</param>
    /// <param name="description">产品描述</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="remark">备注</param>
    /// <param name="attributes">属性值</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品聚合</returns>
    Task<Spu> ImportCreateAsync(string productType, string? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改产品(按Id查询后修改)
    /// </summary>
    /// <param name="id">产品Id</param>
    /// <param name="group">组别</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">产品代码</param>
    /// <param name="name">产品名称</param>
    /// <param name="description">产品描述</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="remark">备注</param>
    /// <param name="attributes">属性值</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品聚合</returns>
    Task<Spu> ModifyAsync(long id, string? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改产品(使用已有聚合)
    /// </summary>
    /// <param name="spu">产品聚合</param>
    /// <param name="group">组别</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">产品代码</param>
    /// <param name="name">产品名称</param>
    /// <param name="description">产品描述</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="remark">备注</param>
    /// <param name="attributes">属性值</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(Spu spu, string? group, long supplierId, long? brandId, string code, string name, string? description, decimal purchasePrice, long currencyId, long unitId, long? taxRateId, decimal? moq, string? remark, SortedDictionary<string, object?>? attributes, IReadOnlyCollection<long>? companyIds, IReadOnlyCollection<long>? attachmentIds, CancellationToken cancellationToken = default);
}
