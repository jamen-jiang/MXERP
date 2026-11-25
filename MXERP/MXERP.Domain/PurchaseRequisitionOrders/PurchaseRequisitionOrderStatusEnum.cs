using System.ComponentModel;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单状态
/// </summary>
public enum PurchaseRequisitionOrderStatusEnum
{
    /// <summary>
    /// 待提交
    /// </summary>
    [Description("待提交")]
    Pending,

    /// <summary>
    /// 待审批
    /// </summary>
    [Description("待审批")]
    PendingApproval,

    /// <summary>
    /// 待采购审核
    /// </summary>
    [Description("待采购审核")]
    PendingPurchaseCheck,

    /// <summary>
    /// 待发货
    /// </summary>
    [Description("待发货")]
    Delivery,

    /// <summary>
    /// 部分发货
    /// </summary>
    [Description("部分发货")]
    PartialDelivered,

    /// <summary>
    /// 全部发货
    /// </summary>
    [Description("全部发货")]
    AllDelivered,

    /// <summary>
    /// 超出发货
    /// </summary>
    [Description("超出发货")]
    OverDelivered,

    /// <summary>
    /// 已完成
    /// </summary>
    [Description("已完成")]
    Finished,

    /// <summary>
    /// 审批拒绝
    /// </summary>
    [Description("审批拒绝")]
    ApprovalRejected,

    /// <summary>
    /// 采购审核拒绝
    /// </summary>
    [Description("采购审核拒绝")]
    PurchaseCheckRejected,

    /// <summary>
    /// 已取消
    /// </summary>
    [Description("已取消")]
    Canceled
}
