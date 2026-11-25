using Snow.Domain.Repositories;

namespace MXERP.Domain.Stocks;

/// <summary>
/// 库存仓储接口
/// </summary>
public interface IStockRepository : IRepository<Stock>
{
    /// <summary>
    /// 按SkuId + 仓库Id查询库存, 不存在返回 null
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存, 不存在返回 null</returns>
    Task<Stock?> GetBySkuIdAndWarehouseIdAsync(long skuId, long warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按SkuId查询库存列表
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存列表</returns>
    Task<List<Stock>> GetListBySkuIdAsync(long skuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断SkuId + 仓库Id是否存在
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistBySkuIdAndWarehouseIdAsync(long skuId, long warehouseId, CancellationToken cancellationToken = default);
}
