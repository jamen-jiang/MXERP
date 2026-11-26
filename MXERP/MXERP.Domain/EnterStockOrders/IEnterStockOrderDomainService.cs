using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单领域服务接口
/// <para>负责入库单项同步、红冲入库单生成、采购单生成入库单、盘点盈利生成入库单、出库批次生成入库单以及冲销数量更新等入库单领域逻辑。</para>
/// </summary>
public interface IEnterStockOrderDomainService
{
    /// <summary>
    /// 同步入库单项列表
    /// </summary>
    /// <param name="enterStockOrder">入库单聚合</param>
    /// <param name="skuInfos">SKU信息列表（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SyncEnterStockOrderItemsAsync(EnterStockOrder enterStockOrder, IReadOnlyCollection<SkuInfoData>? skuInfos, CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成入库单（红冲）
    /// </summary>
    /// <param name="enterStockOrderId">入库单Id</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="skuInfos">SKU信息列表</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单</returns>
    Task<EnterStockOrder> GenerateAsync(long enterStockOrderId, string? remark, IReadOnlyCollection<SkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从采购单生成
    /// </summary>
    /// <param name="purchaseOrderId">采购单Id</param>
    /// <param name="skuInfos">SKU信息列表</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单</returns>
    Task<EnterStockOrder> GenerateFromPurchaseOrderAsync(long purchaseOrderId, IReadOnlyCollection<UniqueSkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 盘点单生成入库单
    /// </summary>
    /// <param name="inventoryOrderId">盘点单Id</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单，无盈利时返回 null</returns>
    Task<EnterStockOrder?> GenerateFromInventoryOrderAsync(long inventoryOrderId, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 出库批次生成入库单
    /// </summary>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注</param>
    /// <param name="skuInfos">SKU信息列表</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单</returns>
    Task<EnterStockOrder> GenerateFromLeaveStockBatchAsync(long leaveStockBatchId, string businessType, string remark, IReadOnlyCollection<SkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新冲销数量
    /// </summary>
    /// <param name="reversalEnterStockOrderId">红冲入库单Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>原入库单，无 OriginalId 时返回 null</returns>
    Task<EnterStockOrder?> UpdateReversalQtyAsync(long reversalEnterStockOrderId, CancellationToken cancellationToken = default);
}