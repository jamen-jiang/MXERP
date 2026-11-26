using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单关联表
/// </summary>
public class LeaveStockOrderRelation : DeletionAuditedEntity
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveStockOrderRelation()
    {
        RelationNumber = string.Empty;
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="qty">数量</param>
    public LeaveStockOrderRelation(string relationType, long relationId, long relationItemId, string relationNumber, double qty)
    {
        RelationType = relationType;
        RelationId = relationId;
        RelationItemId = relationItemId;
        RelationNumber = relationNumber;
        Qty = qty;
    }

    /// <summary>
    /// 出库单项Id
    /// </summary>
    public long? LeaveStockOrderItemId { get; private set; }

    /// <summary>
    /// 关联类型
    /// </summary>
    public string RelationType { get; private set; } = string.Empty;

    /// <summary>
    /// 关联Id
    /// </summary>
    public long RelationId { get; private set; }

    /// <summary>
    /// 关联项Id
    /// </summary>
    public long RelationItemId { get; private set; }

    /// <summary>
    /// 关联号
    /// </summary>
    public string RelationNumber { get; private set; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
}
