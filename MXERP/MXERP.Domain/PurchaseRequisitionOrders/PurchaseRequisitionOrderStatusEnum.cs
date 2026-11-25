namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单状态
/// </summary>
public static class PurchaseRequisitionOrderStatusEnum
{
    /// <summary>
    /// 待提交
    /// </summary>
    public const string Pending = "Pending";

    /// <summary>
    /// 待审批
    /// </summary>
    public const string PendingApproval = "PendingApproval";

    /// <summary>
    /// 待采购审核
    /// </summary>
    public const string PendingPurchaseCheck = "PendingPurchaseCheck";

    /// <summary>
    /// 待发货
    /// </summary>
    public const string Delivery = "Delivery";

    /// <summary>
    /// 部分发货
    /// </summary>
    public const string PartialDelivered = "PartialDelivered";

    /// <summary>
    /// 全部发货
    /// </summary>
    public const string AllDelivered = "AllDelivered";

    /// <summary>
    /// 超出发货
    /// </summary>
    public const string OverDelivered = "OverDelivered";

    /// <summary>
    /// 已完成
    /// </summary>
    public const string Finished = "Finished";

    /// <summary>
    /// 审批拒绝
    /// </summary>
    public const string ApprovalRejected = "ApprovalRejected";

    /// <summary>
    /// 采购审核拒绝
    /// </summary>
    public const string PurchaseCheckRejected = "PurchaseCheckRejected";

    /// <summary>
    /// 已取消
    /// </summary>
    public const string Canceled = "Canceled";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Pending,
        PendingApproval,
        PendingPurchaseCheck,
        Delivery,
        PartialDelivered,
        AllDelivered,
        OverDelivered,
        Finished,
        ApprovalRejected,
        PurchaseCheckRejected,
        Canceled
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>true 表示有效</returns>
    public static bool IsValid(string? code)
    {
        if (code is null)
        {
            return false;
        }

        return All.Contains(code);
    }
}
