using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项领域服务接口
/// </summary>
public interface IBulkOrderItemDomainService
{
    /// <summary>
    /// 创建批量订单项
    /// </summary>
    /// <param name="bulkOrderId">批量订单Id</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id(可空)</param>
    /// <param name="customerSpuCode">客人产品Code(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="packetInfo">包装信息(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="attachmentIds">附件Id集合(可空)</param>
    /// <param name="attachments">附件集合(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单项</returns>
    Task<BulkOrderItem> CreateAsync(long bulkOrderId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, string? packetInfo, string? remark, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改批量订单项(按Id查询后修改)
    /// </summary>
    /// <param name="id">批量订单项Id</param>
    /// <param name="customerSpuId">客人产品Id(可空)</param>
    /// <param name="customerSpuCode">客人产品Code(可空)</param>
    /// <param name="packetInfo">包装信息(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="attachmentIds">附件Id集合(可空)</param>
    /// <param name="attachments">附件集合(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单项</returns>
    Task<BulkOrderItem> ModifyAsync(long id, long? customerSpuId, string? customerSpuCode, string? packetInfo, string? remark, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消确认
    /// </summary>
    /// <param name="id">批量订单项Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>批量订单项</returns>
    Task<BulkOrderItem> UnConfirmAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 同步销售SKU行列表
    /// </summary>
    /// <param name="item">批量订单项</param>
    /// <param name="salesSkus">目标销售SKU行列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task SyncSalesSkusAsync(BulkOrderItem item, IReadOnlyCollection<BulkOrderItemSalesSkuData>? salesSkus, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配批量订单项
    /// </summary>
    /// <param name="id">批量订单项Id</param>
    /// <param name="leaveStockPlanSkuInfos">出库计划Sku信息列表</param>
    /// <param name="purchasePlanSkuInfos">采购计划Sku信息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    Task<BulkOrderItemAssignResult> AssignItemAsync(long id, IReadOnlyCollection<SkuInfoData> leaveStockPlanSkuInfos, IReadOnlyCollection<SkuInfoData> purchasePlanSkuInfos, CancellationToken cancellationToken = default);

    /// <summary>
    /// 自动分配单项(按Id)
    /// </summary>
    /// <param name="id">批量订单项Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    Task<BulkOrderItemAssignResult> AutomaticAssignItemAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 自动分配单项(按聚合)
    /// </summary>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    Task<BulkOrderItemAssignResult> AutomaticAssignItemAsync(BulkOrderItem bulkOrderItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// 全订购分配单项(按Id)
    /// </summary>
    /// <param name="id">批量订单项Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    Task<BulkOrderItemAssignResult> AllCustomizedAssignItemAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 全订购分配单项(按聚合)
    /// </summary>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    Task<BulkOrderItemAssignResult> AllCustomizedAssignItemAsync(BulkOrderItem bulkOrderItem, CancellationToken cancellationToken = default);
}
