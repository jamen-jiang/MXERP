using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单项
/// </summary>
public class LeaveWarehouseOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveWarehouseOrderItem()
    {
        LeaveStockBatchNumber = string.Empty;
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="leaveStockBatchNumber">出库批次号</param>
    /// <param name="salesSpuId">销售产品Id(可空)</param>
    /// <param name="salesSpuCode">销售产品Code(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="remark">备注(可空)</param>
    internal LeaveWarehouseOrderItem(long leaveStockBatchId, string leaveStockBatchNumber, long? salesSpuId, string? salesSpuCode, long spuId, double qty, decimal price, string? remark)
    {
        LeaveStockBatchId = leaveStockBatchId;
        LeaveStockBatchNumber = leaveStockBatchNumber;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        SpuId = spuId;
        Qty = qty;
        Price = price;
        Remark = remark;
    }

    /// <summary>
    /// 出仓单Id
    /// </summary>
    public long? LeaveWarehouseOrderId { get; private set; }

    /// <summary>
    /// 出库批次Id
    /// </summary>
    public long LeaveStockBatchId { get; private set; }

    /// <summary>
    /// 出库批次号
    /// </summary>
    public string LeaveStockBatchNumber { get; private set; }

    /// <summary>
    /// 销售产品Id(可空)
    /// </summary>
    public long? SalesSpuId { get; private set; }

    /// <summary>
    /// 销售产品Code(可空)
    /// </summary>
    public string? SalesSpuCode { get; private set; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 出仓数量
    /// </summary>
    public double Qty { get; private set; }

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 修改备注
    /// </summary>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(string? remark)
    {
        Remark = remark;
    }
}
