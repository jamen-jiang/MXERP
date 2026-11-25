using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单领域服务接口
/// </summary>
public interface ILeaveStockOrderDomainService
{
    /// <summary>
    /// 同步出库单项列表
    /// </summary>
    /// <param name="leaveStockOrder">出库单聚合</param>
    /// <param name="skuInfos">SKU信息列表（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncLeaveStockOrderItemsAsync(LeaveStockOrder leaveStockOrder, IReadOnlyCollection<SkuInfoData>? skuInfos, CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成红冲出库单
    /// </summary>
    /// <param name="leaveStockOrderId">出库单Id</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="skuInfos">SKU信息列表</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>红冲出库单</returns>
    Task<LeaveStockOrder> GenerateAsync(long leaveStockOrderId, string? remark, IReadOnlyCollection<SkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 出库批次生成出库单
    /// </summary>
    /// <param name="datas">出库批次生成参数列表</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单</returns>
    Task<LeaveStockOrder> GenerateFromLeaveStockBatchesAsync(IReadOnlyCollection<GenerateLeaveStockOrderFromBatchData> datas, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 出库批次生成出库单
    /// </summary>
    /// <param name="batchNumber">出库批次号</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单</returns>
    Task<LeaveStockOrder> GenerateFromLeaveStockBatchNumberAsync(string batchNumber, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 盘点单生成出库单
    /// </summary>
    /// <param name="inventoryOrderId">盘点单Id</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单，无亏损时返回 null</returns>
    Task<LeaveStockOrder?> GenerateFromInventoryOrderAsync(long inventoryOrderId, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从散镜订单批次生成出库单
    /// </summary>
    /// <param name="retailOrderNumber">散镜订单号</param>
    /// <param name="retailOrderItemIdR">散镜订单项IdR</param>
    /// <param name="spuIdR">产品IdR</param>
    /// <param name="warehouseIdR">仓库IdR</param>
    /// <param name="retailOrderItemIdL">散镜订单项IdL</param>
    /// <param name="spuIdL">产品IdL</param>
    /// <param name="warehouseIdL">仓库IdL</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库单列表</returns>
    Task<List<LeaveStockOrder>> GenerateFromRetailOrderBatchAsync(string retailOrderNumber, long retailOrderItemIdR, long spuIdR, long warehouseIdR, long retailOrderItemIdL, long spuIdL, long warehouseIdL, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新冲销数量
    /// </summary>
    /// <param name="reversalLeaveStockOrderId">红冲出库单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>原出库单，无 OriginalId 时返回 null</returns>
    Task<LeaveStockOrder?> UpdateReversalQtyAsync(long reversalLeaveStockOrderId, CancellationToken cancellationToken = default);
}
