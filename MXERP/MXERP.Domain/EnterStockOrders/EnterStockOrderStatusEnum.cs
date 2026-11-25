namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单状态
/// </summary>
public static class EnterStockOrderStatusEnum
{
    /// <summary>
    /// 待入库
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
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Pending,
        Finished,
        Canceled
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>是否有效</returns>
    public static bool IsValid(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && All.Contains(code);
    }
}
