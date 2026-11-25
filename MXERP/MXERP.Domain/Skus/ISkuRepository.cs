using Snow.Domain.Repositories;

namespace MXERP.Domain.Skus;

/// <summary>
/// SKU仓储接口
/// </summary>
public interface ISkuRepository : IRepository<Sku>
{
    /// <summary>
    /// 按Id列表查询
    /// </summary>
    /// <param name="ids">Id列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU列表</returns>
    Task<List<Sku>> GetListByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按属性标识码查询
    /// </summary>
    /// <param name="attributesSignature">属性标识码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU, 不存在返回 null</returns>
    Task<Sku?> GetByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按SpuId查询列表
    /// </summary>
    /// <param name="spuId">SpuId</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>SKU列表</returns>
    Task<List<Sku>> GetListBySpuIdAsync(long spuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断属性标识码是否存在
    /// </summary>
    /// <param name="attributesSignature">属性标识码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示存在</returns>
    Task<bool> IsExistByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断OPC是否存在(排除指定Id)
    /// </summary>
    /// <param name="spuId">SpuId</param>
    /// <param name="opc">OPC</param>
    /// <param name="id">排除的SKU Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示存在</returns>
    Task<bool> IsExistBySpuIdAndOpcExcludeIdAsync(long spuId, string opc, long? id = null, CancellationToken cancellationToken = default);
}
