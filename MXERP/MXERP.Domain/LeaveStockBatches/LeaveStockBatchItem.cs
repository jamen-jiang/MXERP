using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次项
/// </summary>
public class LeaveStockBatchItem : DeletionAuditedEntity
{
    /// <summary>
    /// 出库批次Id
    /// </summary>
    public long? LeaveStockBatchId { get; private set; }

    /// <summary>
    /// 采购批次项Id（可空）
    /// </summary>
    public long? PurchaseBatchItemId { get; private set; }

    /// <summary>
    /// 关联项Id
    /// </summary>
    public long RelationItemId { get; private set; }

    /// <summary>
    /// 销售产品Id（可空）
    /// </summary>
    public long? SalesSpuId { get; private set; }

    /// <summary>
    /// 销售产品Code（可空）
    /// </summary>
    public string? SalesSpuCode { get; private set; }

    /// <summary>
    /// 客人产品Id（可空）
    /// </summary>
    public long? CustomerSpuId { get; private set; }

    /// <summary>
    /// 客人产品Code（可空）
    /// </summary>
    public string? CustomerSpuCode { get; private set; }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }

    /// <summary>
    /// 产品SkuId
    /// </summary>
    public long SkuId { get; private set; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }

    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }

    /// <summary>
    /// 开单数量
    /// </summary>
    public double BillingQty { get; private set; }

    /// <summary>
    /// Sku 属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }

    /// <summary>
    /// 非关键属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }

    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveStockBatchItem()
    {
    }

    /// <summary>
    /// 构造器（无销售产品）
    /// </summary>
    /// <param name="purchaseBatchItemId">采购批次项Id（可空）</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    internal LeaveStockBatchItem(long? purchaseBatchItemId, long relationItemId, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        PurchaseBatchItemId = purchaseBatchItemId;
        RelationItemId = relationItemId;
        SpuId = spuId;
        SkuId = skuId;
        WarehouseId = warehouseId;
        Qty = qty;
        SkuAttributes = skuAttributes;
        OtherAttributes = otherAttributes;
    }

    /// <summary>
    /// 构造器（带销售产品）
    /// </summary>
    /// <param name="purchaseBatchItemId">采购批次项Id（可空）</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="salesSpuId">销售产品Id（可空）</param>
    /// <param name="salesSpuCode">销售产品Code（可空）</param>
    /// <param name="customerSpuId">客人产品Id（可空）</param>
    /// <param name="customerSpuCode">客人产品Code（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    internal LeaveStockBatchItem(long? purchaseBatchItemId, long relationItemId, long? salesSpuId, string? salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
        : this(purchaseBatchItemId, relationItemId, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes)
    {
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
    }

    /// <summary>
    /// 添加出库数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void AddLeaveStockQty(double qty)
    {
        LeaveStockQty += qty;
        if (LeaveStockQty < 0)
        {
            throw new UserFriendlyException("出库数量不允许小于0!");
        }

        if (LeaveStockQty > Qty)
        {
            throw new UserFriendlyException("出库数量不允许大于批次数量!");
        }
    }

    /// <summary>
    /// 添加开单数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void AddBillingQty(double qty)
    {
        BillingQty += qty;
        if (BillingQty > LeaveStockQty)
        {
            throw new UserFriendlyException("开单数量不允许超出出库数量!");
        }
    }

    /// <summary>
    /// 减去开单数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void SubtractBillingQty(double qty)
    {
        BillingQty -= qty;
        if (BillingQty < 0)
        {
            throw new UserFriendlyException("开单数量不允许小于0!");
        }
    }
}
