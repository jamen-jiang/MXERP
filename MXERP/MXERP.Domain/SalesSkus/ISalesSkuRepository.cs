using Snow.Domain.Repositories;

namespace MXERP.Domain.SalesSkus;

/// <summary>
/// 销售SKU仓储接口
/// </summary>
public interface ISalesSkuRepository : IRepository<SalesSku>
{
    /// <summary>
    /// 按属性标识码查询
    /// </summary>
    /// <param name="attributesSignature">属性标识码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售SKU, 不存在返回 null</returns>
    Task<SalesSku?> GetByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按销售产品Id查询列表
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售SKU列表</returns>
    Task<List<SalesSku>> GetBySalesSpuIdAsync(long salesSpuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断属性标识码是否存在
    /// </summary>
    /// <param name="attributesSignature">属性标识码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示存在</returns>
    Task<bool> IsExistByAttributesSignatureAsync(string attributesSignature, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断公司Id + Opc是否存在(排除指定主键)
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="opc">Opc</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示存在</returns>
    Task<bool> IsExistByCompanyIdAndOpcExcludeIdAsync(long companyId, string opc, long? id = null, CancellationToken cancellationToken = default);
}
