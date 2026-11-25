namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单状态(对应 Java 的 BulkOrderStatusEnum, 以编码字符串存储)
/// </summary>
public static class BulkOrderStatusEnum
{
    /// <summary>
    /// 待发货
    /// </summary>
    public const string Pending = "Pending";

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
    /// 已取消
    /// </summary>
    public const string Canceled = "Canceled";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Pending,
        PartialDelivered,
        AllDelivered,
        OverDelivered,
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
