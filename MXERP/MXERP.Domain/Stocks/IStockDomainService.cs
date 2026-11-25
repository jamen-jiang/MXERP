namespace MXERP.Domain.Stocks;

/// <summary>
/// 库存领域服务接口
/// </summary>
public interface IStockDomainService
{
    /// <summary>
    /// 添加库存（库存不存在时自动创建）
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存, qty 为 0 或不存在时返回 null</returns>
    Task<Stock?> AddQtyAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default);

    /// <summary>
    /// 减去库存
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存, qty 为 0 或不存在时返回 null</returns>
    Task<Stock?> SubtractQtyAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default);

    /// <summary>
    /// 冻结库存
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存, qty 为 0 或不存在时返回 null</returns>
    Task<Stock?> FreezeStockAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default);

    /// <summary>
    /// 重新冻结库存（不做可用量校验）
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存, qty 为 0 或不存在时返回 null</returns>
    Task<Stock?> FreezeStockAgainAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解冻库存
    /// </summary>
    /// <param name="skuId">SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>库存, qty 为 0 或不存在时返回 null</returns>
    Task<Stock?> ThawStockAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default);
}
