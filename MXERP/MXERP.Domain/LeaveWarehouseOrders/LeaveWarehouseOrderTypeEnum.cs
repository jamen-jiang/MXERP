namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单类型
/// </summary>
public static class LeaveWarehouseOrderTypeEnum
{
    /// <summary>
    /// 正常出仓
    /// </summary>
    public const string Normal = "Normal";

    /// <summary>
    /// 借调出仓
    /// </summary>
    public const string Second = "Second";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Normal,
        Second
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
