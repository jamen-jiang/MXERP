namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单项数据
/// </summary>
public class LeaveWarehouseOrderItemData
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="salesSpuId">销售产品Id(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="qty">数量</param>
    /// <param name="remark">备注(可空)</param>
    public LeaveWarehouseOrderItemData(long leaveStockBatchId, long? salesSpuId, long spuId, double qty, string? remark)
    {
        LeaveStockBatchId = leaveStockBatchId;
        SalesSpuId = salesSpuId;
        SpuId = spuId;
        Qty = qty;
        Remark = remark;
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

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; }
}
