namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次领域服务接口
/// </summary>
public interface IPurchaseBatchDomainService
{
    /// <summary>
    /// 从批量订单项生成采购批次
    /// </summary>
    /// <param name="bulkOrderItemId">批量订单项Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次列表(无采购计划时返回空列表)</returns>
/// <summary>
    /// 从审核申购单生成采购批次
    /// </summary>
    /// <param name="purchaseRequisitionOrderId">申购单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次, 无采购计划时返回 null</returns>
    Task<PurchaseBatch?> GenerateFromCheckPurchaseRequisitionOrderAsync(long purchaseRequisitionOrderId, CancellationToken cancellationToken = default);

    Task<List<PurchaseBatch>> GenerateFromBulkOrderItemAsync(long bulkOrderItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从出库批次生成采购批次
    /// </summary>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次列表</returns>
    Task<List<PurchaseBatch>> GenerateFromLeaveStockBatchAsync(long leaveStockBatchId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 弃审采购批次
    /// </summary>
    /// <param name="id">采购批次Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购批次</returns>
    Task<PurchaseBatch> UnCheckAsync(long id, string? remark, CancellationToken cancellationToken = default);
}
