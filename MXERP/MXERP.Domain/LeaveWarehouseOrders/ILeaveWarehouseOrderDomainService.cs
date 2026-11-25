namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单领域服务接口
/// </summary>
public interface ILeaveWarehouseOrderDomainService
{
    /// <summary>
    /// 同步出仓单项列表
    /// </summary>
    /// <param name="leaveWarehouseOrder">出仓单聚合</param>
    /// <param name="leaveWarehouseOrderItems">出仓单项数据列表(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task SyncLeaveWarehouseOrderItemsAsync(LeaveWarehouseOrder leaveWarehouseOrder, IReadOnlyCollection<LeaveWarehouseOrderItemData>? leaveWarehouseOrderItems, CancellationToken cancellationToken = default);
}
