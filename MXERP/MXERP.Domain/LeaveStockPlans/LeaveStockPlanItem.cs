using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.LeaveStockPlans;

/// <summary>
/// 出库计划项
/// </summary>
public class LeaveStockPlanItem : DeletionAuditedEntity
{
    /// <summary>
    /// 出库计划Id
    /// </summary>
    public long? LeaveStockPlanId { get; private set; }
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
    protected LeaveStockPlanItem()
    {
    }

    /// <summary>
    /// 构造器（无销售产品）
    /// </summary>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    internal LeaveStockPlanItem(long relationItemId, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
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
    /// <param name="relationItemId">关联项Id</param>
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
    internal LeaveStockPlanItem(long relationItemId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
        : this(relationItemId, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes)
    {
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
    }
}
