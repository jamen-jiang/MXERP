using MXERP.Domain.Common.Enums;
using Snow.Domain.Events;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次项新增事件
/// </summary>
public class LeaveStockBatchItemAddedEvent : DomainEvent
{
    /// <summary>
    /// 出库计划Id（可空）
    /// </summary>
    public long? LeaveStockPlanId { get; }

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
    /// 采购批次项Id（可空）
    /// </summary>
    public long? PurchaseBatchItemId { get; }

    /// <summary>
    /// 关联项Id
    /// </summary>
    public long ItemRelationItemId { get; }

    /// <summary>
    /// 关联号
    /// </summary>
    public string RelationNumber { get; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockBatch">出库批次</param>
    /// <param name="leaveStockPlanId">出库计划Id（可空）</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="purchaseBatchItemId">采购批次项Id（可空）</param>
    /// <param name="itemRelationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="qty">数量</param>
    public LeaveStockBatchItemAddedEvent(LeaveStockBatch leaveStockBatch, long? leaveStockPlanId, string relationType, long relationId, long? relationItemId, long? purchaseBatchItemId, long itemRelationItemId, string relationNumber, double qty)
        : base(leaveStockBatch)
    {
        LeaveStockPlanId = leaveStockPlanId;
        RelationType = relationType;
        RelationId = relationId;
        RelationItemId = relationItemId;
        PurchaseBatchItemId = purchaseBatchItemId;
        ItemRelationItemId = itemRelationItemId;
        RelationNumber = relationNumber;
        Qty = qty;
    }
}
