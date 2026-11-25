using Snow.Domain.Repositories;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项仓储接口
/// </summary>
public interface IBulkOrderItemRepository : IRepository<BulkOrderItem>
{
    /// <summary>
    /// 按批量订单Id查询列表
    /// </summary>
    /// <param name="bulkOrderId">批量订单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单项列表</returns>
    Task<List<BulkOrderItem>> GetListByBulkOrderIdAsync(long bulkOrderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断批量订单下销售产品是否存在
    /// </summary>
    /// <param name="bulkOrderId">批量订单Id</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="id">排除的主键Id(可空), 修改时排除自身</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByBulkOrderIdAndSalesSpuIdExcludeIdAsync(long bulkOrderId, long salesSpuId, long? id = null, CancellationToken cancellationToken = default);
}
