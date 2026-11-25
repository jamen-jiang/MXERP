namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 任务类型
/// </summary>
public static class TaskEnum
{
    /// <summary>
    /// 审核
    /// </summary>
    public const string Check = "Check";

    /// <summary>
    /// 批准
    /// </summary>
    public const string Approve = "Approve";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Check,
        Approve
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
