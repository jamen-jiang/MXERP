using Snow.Domain.Events;

namespace MXERP.Domain.PurchasePlans;

/// <summary>
/// 采购计划项新增事件
/// </summary>
public class PurchasePlanItemAddedEvent : DomainEvent
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchasePlan">采购计划</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="itemRelationItemId">采购计划项关联项Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    public PurchasePlanItemAddedEvent(PurchasePlan purchasePlan, string relationType, long relationId, long? relationItemId, long itemRelationItemId, long skuId, long warehouseId, double qty)
        : base(purchasePlan)
    {
        RelationType = relationType;
        RelationId = relationId;
        RelationItemId = relationItemId;
        ItemRelationItemId = itemRelationItemId;
        SkuId = skuId;
        WarehouseId = warehouseId;
        Qty = qty;
    }

    /// <summary>
    /// 关联类型
    /// </summary>
    public string RelationType { get; }

    /// <summary>
    /// 关联Id
    /// </summary>
    public long RelationId { get; }

    /// <summary>
    /// 关联项Id（可空）
    /// </summary>
    public long? RelationItemId { get; }

    /// <summary>
    /// 采购计划项关联项Id
    /// </summary>
    public long ItemRelationItemId { get; }

    /// <summary>
    /// 产品SkuId
    /// </summary>
    public long SkuId { get; }

    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; }
}
