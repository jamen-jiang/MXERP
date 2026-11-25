namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单状态(对应 Java 的 ExternalLeaveStockOrderStatusEnum, 以编码字符串存储)
/// </summary>
public static class ExternalLeaveStockOrderStatusEnum
{
    /// <summary>
    /// 待完成
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
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Pending,
        Finished,
        Canceled
    };

    /// <summary>
    /// 编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        return code is not null && All.Contains(code);
    }
}
