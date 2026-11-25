namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库业务类型
/// </summary>
public static class LeaveStockBusinessTypeEnum
{
    /// <summary>
    /// 销售出库
    /// </summary>
    public const string SalesLeaveStock = "SalesLeaveStock";

    /// <summary>
    /// 订单出库
    /// </summary>
    public const string OrderLeaveStock = "OrderLeaveStock";

    /// <summary>
    /// 盘点出库
    /// </summary>
    public const string InventoryLeaveStock = "InventoryLeaveStock";

    /// <summary>
    /// 转仓出库
    /// </summary>
    public const string TransferLeaveStock = "TransferLeaveStock";

    /// <summary>
    /// 冲销出库
    /// </summary>
    public const string ReversalLeaveStock = "ReversalLeaveStock";

    /// <summary>
    /// 全部业务类型
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        SalesLeaveStock,
        OrderLeaveStock,
        InventoryLeaveStock,
        TransferLeaveStock,
        ReversalLeaveStock
    };

    /// <summary>
    /// 判断业务类型是否合法
    /// </summary>
    /// <param name="code">业务类型编码</param>
    /// <returns>true：合法</returns>
    public static bool IsValid(string? code)
    {
        return !string.IsNullOrWhiteSpace(code) && All.Contains(code);
    }
}
