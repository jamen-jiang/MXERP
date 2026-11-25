using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.PurchasePlans;

/// <summary>
/// 采购计划项数据
/// </summary>
public class PurchasePlanItemData : SkuInfoData
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="salesSpuId">销售产品Id（可空）</param>
    /// <param name="salesSpuCode">销售产品Code（可空）</param>
    /// <param name="customerSpuId">客人产品Id（可空）</param>
    /// <param name="customerSpuCode">客人产品Code（可空）</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public PurchasePlanItemData(long relationItemId, long? salesSpuId, string? salesSpuCode, long? customerSpuId, string? customerSpuCode, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
        : base(skuId, warehouseId, qty)
    {
        RelationItemId = relationItemId;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        CustomerSpuId = customerSpuId;
        CustomerSpuCode = customerSpuCode;
        SkuAttributes = skuAttributes;
        OtherAttributes = otherAttributes;
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    public PurchasePlanItemData(long relationItemId, long skuId, long warehouseId, double qty)
        : this(relationItemId, null, null, null, null, skuId, warehouseId, qty, null, null)
    {
    }

    /// <summary>
    /// 关联项Id
    /// </summary>
    public long RelationItemId { get; }

    /// <summary>
    /// 销售产品Id（可空）
    /// </summary>
    public long? SalesSpuId { get; }

    /// <summary>
    /// 销售产品Code（可空）
    /// </summary>
    public string? SalesSpuCode { get; }

    /// <summary>
    /// 客人产品Id（可空）
    /// </summary>
    public long? CustomerSpuId { get; }

    /// <summary>
    /// 客人产品Code（可空）
    /// </summary>
    public string? CustomerSpuCode { get; }

    /// <summary>
    /// Sku属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; }

    /// <summary>
    /// 非关键属性值（可空）
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; }
}
