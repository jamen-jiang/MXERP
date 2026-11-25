namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单状态
/// </summary>
public static class LeaveStockOrderStatusEnum
{
    /// <summary>
    /// 待出库
    /// </summary>
    public const string Pending = "Pending";

    /// <summary>
    /// 已完成
    /// </summary>
    public const string Finished = "Finished";

    /// <summary>
    /// 已取消
    /// </summary>
    public const string Canceled = "Canceled";

    /// <summary>
    /// 全部状态
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Pending,
        Finished,
        Canceled
    };

    /// <summary>
    /// 判断状态是否合法
    /// </summary>
    /// <param name="code">状态编码</param>
    /// <returns>true：合法</returns>
    public static bool IsValid(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && All.Contains(code);
    }
}
