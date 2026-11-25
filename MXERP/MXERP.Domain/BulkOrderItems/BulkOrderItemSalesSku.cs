using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项的销售SKU行(唯一业务标识: SalesSkuId)
/// </summary>
public class BulkOrderItemSalesSku : DeletionAuditedEntity
{
    /// <summary>
    /// 批量订单项Id
    /// </summary>
    public long? BulkOrderItemId { get; private set; }
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long SalesSpuId { get; private set; }
    /// <summary>
    /// 销售SkuId
    /// </summary>
    public long SalesSkuId { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 抽仓数量
    /// </summary>
    public double DeductQty { get; private set; }
    /// <summary>
    /// 订购数量
    /// </summary>
    public double CustomizedQty { get; private set; }
    /// <summary>
    /// 已出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }
    /// <summary>
    /// Sku关键属性值(可空)
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }
    /// <summary>
    /// 非关键属性值(可空)
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected BulkOrderItemSalesSku()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSkuId">销售SkuId</param>
    /// <param name="skuAttributes">Sku关键属性值(可空)</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    /// <param name="qty">数量</param>
    internal BulkOrderItemSalesSku(long salesSpuId, long salesSkuId, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes, double qty)
    {
        SalesSpuId = salesSpuId;
        SalesSkuId = salesSkuId;
        SkuAttributes = skuAttributes;
        OtherAttributes = otherAttributes;
        Qty = qty;
    }

    /// <summary>
    /// 修改(不允许改 SkuAttributes, 改动等于换 SKU)
    /// </summary>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    /// <param name="qty">数量</param>
    internal void Modify(SortedDictionary<string, object?>? otherAttributes, double qty)
    {
        OtherAttributes = otherAttributes;
        Qty = qty;
    }

    /// <summary>
    /// 欠数
    /// </summary>
    /// <returns>欠数</returns>
    public double GetOweQty()
    {
        return Qty - LeaveStockQty;
    }

    /// <summary>
    /// 未分配数量
    /// </summary>
    /// <returns>未分配数量</returns>
    public double GetNoAssignQty()
    {
        return Qty - DeductQty - CustomizedQty;
    }

    /// <summary>
    /// 是否已分配
    /// </summary>
    /// <returns>true 表示已分配</returns>
    public bool GetIsAssign()
    {
        return Math.Abs(DeductQty + CustomizedQty - Qty) < 1e-9;
    }

    /// <summary>
    /// 更新出库数量
    /// </summary>
    /// <param name="qty">出库数量</param>
    internal void UpdateLeaveStockQty(double qty)
    {
        LeaveStockQty += qty;
    }

    /// <summary>
    /// 更新抽仓数量
    /// </summary>
    /// <param name="qty">抽仓数量</param>
    internal void UpdateDeductQty(double qty)
    {
        DeductQty += qty;
    }

    /// <summary>
    /// 更新订购数量
    /// </summary>
    /// <param name="qty">订购数量</param>
    internal void UpdateCustomizedQty(double qty)
    {
        CustomizedQty += qty;
    }
}
