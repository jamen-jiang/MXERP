namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库业务类型
/// </summary>
public static class EnterStockBusinessTypeEnum
{
    /// <summary>
    /// 采购入库
    /// </summary>
    public const string PurchaseEnterStock = "PurchaseEnterStock";

    /// <summary>
    /// 订单入库
    /// </summary>
    public const string OrderEnterStock = "OrderEnterStock";

    /// <summary>
    /// 盘点入库
    /// </summary>
    public const string InventoryEnterStock = "InventoryEnterStock";

    /// <summary>
    /// 转仓入库
    /// </summary>
    public const string TransferEnterStock = "TransferEnterStock";

    /// <summary>
    /// 冲销出库
    /// </summary>
    public const string ReversalEnterStock = "ReversalEnterStock";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        PurchaseEnterStock,
        OrderEnterStock,
        InventoryEnterStock,
        TransferEnterStock,
        ReversalEnterStock
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
