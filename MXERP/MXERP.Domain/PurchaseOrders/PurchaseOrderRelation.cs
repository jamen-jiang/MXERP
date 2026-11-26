using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单关联表
/// </summary>
public class PurchaseOrderRelation : DeletionAuditedEntity
{
    /// <summary>
    /// 采购单项Id
    /// </summary>
    public long? PurchaseOrderItemId { get; private set; }
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
    public string RelationNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected PurchaseOrderRelation()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="qty">数量</param>
    internal PurchaseOrderRelation(string relationType, long relationId, long relationItemId, string relationNumber, double qty)
    {
        RelationType = relationType;
        RelationId = relationId;
        RelationItemId = relationItemId;
        RelationNumber = relationNumber;
        Qty = qty;
    }
}
