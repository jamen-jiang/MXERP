using Snow.Domain.Events;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单项移除事件
/// </summary>
public class LeaveWarehouseOrderItemDeletedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveWarehouseOrder">出仓单</param>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="salesSpuId">销售产品Id(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="qty">数量</param>
    public LeaveWarehouseOrderItemDeletedEvent(LeaveWarehouseOrder leaveWarehouseOrder, long leaveStockBatchId, long? salesSpuId, long spuId, double qty) : base(leaveWarehouseOrder)
    {
        LeaveStockBatchId = leaveStockBatchId;
        SalesSpuId = salesSpuId;
        SpuId = spuId;
        Qty = qty;
    }

    /// <summary>
    /// 出库批次Id
    /// </summary>
    public long LeaveStockBatchId { get; }

    /// <summary>
    /// 销售产品Id(可空)
    /// </summary>
    public long? SalesSpuId { get; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; }
}
