namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次状态
/// </summary>
public static class PurchaseBatchStatusEnum
{
    /// <summary>
    /// 待采购
    /// </summary>
    public const string Pending = "Pending";
    /// <summary>
    /// 部分采购
    /// </summary>
    public const string PartialPurchased = "PartialPurchased";
    /// <summary>
    /// 全部采购
    /// </summary>
    public const string AllPurchased = "AllPurchased";
    /// <summary>
    /// 超出采购
    /// </summary>
    public const string OverPurchased = "OverPurchased";
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
        PartialPurchased,
        AllPurchased,
        OverPurchased,
        Finished,
        Canceled
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        return code is not null && All.Contains(code);
    }
}
