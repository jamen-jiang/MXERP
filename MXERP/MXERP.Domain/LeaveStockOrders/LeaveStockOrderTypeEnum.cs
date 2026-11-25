namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单类型
/// </summary>
public static class LeaveStockOrderTypeEnum
{
    /// <summary>
    /// 抽仓
    /// </summary>
    public const string Deduct = "Deduct";

    /// <summary>
    /// 订购
    /// </summary>
    public const string Customized = "Customized";

    /// <summary>
    /// 全部类型
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Deduct,
        Customized
    };

    /// <summary>
    /// 判断类型是否合法
    /// </summary>
    /// <param name="code">类型编码</param>
    /// <returns>true：合法</returns>
    public static bool IsValid(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && All.Contains(code);
    }
}
