using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次项
/// </summary>
public class PurchaseBatchItem : DeletionAuditedEntity
{
    /// <summary>
    /// 采购批次Id
    /// </summary>
    public long? PurchaseBatchId { get; private set; }
    /// <summary>
    /// 关联项Id（可空）
    /// </summary>
    public long? RelationItemId { get; private set; }
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
    /// 采购数量
    /// </summary>
    public double PurchaseQty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }
    /// <summary>
    /// Sku属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }
    /// <summary>
    /// 非关键属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected PurchaseBatchItem()
    {
    }

    /// <summary>
    /// 构造器（无销售产品）
    /// </summary>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    internal PurchaseBatchItem(long? relationItemId, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
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
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id（可空）</param>
    /// <param name="customerSpuCode">客人产品Code（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    internal PurchaseBatchItem(long? relationItemId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
        : this(relationItemId, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes)
    {
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
    }

    /// <summary>
    /// 更新采购数量（正数=添加，负数=减去）
    /// </summary>
    /// <param name="qty">数量</param>
    internal void UpdatePurchaseQty(double qty)
    {
        PurchaseQty += qty;
        if (PurchaseQty < 0)
        {
            throw new UserFriendlyException("采购批次采购数量不允许小于0!");
        }

        if (PurchaseQty > Qty)
        {
            throw new UserFriendlyException("采购批次采购数量不允许大于采购批次数量!");
        }
    }

    /// <summary>
    /// 更新出库数量（正数=添加，负数=减去）
    /// </summary>
    /// <param name="qty">数量</param>
    internal void UpdateLeaveStockQty(double qty)
    {
        LeaveStockQty += qty;
        if (LeaveStockQty < 0)
        {
            throw new UserFriendlyException("采购批次出库数量不允许小于0!");
        }

        if (LeaveStockQty > Qty)
        {
            throw new UserFriendlyException("采购批次出库数量不允许大于采购批次数量!");
        }
    }
}
