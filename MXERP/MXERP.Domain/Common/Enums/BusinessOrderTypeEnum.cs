namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 业务单类型枚举
/// </summary>
public static class BusinessOrderTypeEnum
{
    /// <summary>
    /// 采购单
    /// </summary>
    public const string PurchaseOrder = "PurchaseOrder";

    /// <summary>
    /// 调拨单
    /// </summary>
    public const string TransferOrder = "TransferOrder";

    /// <summary>
    /// 镜种转换单
    /// </summary>
    public const string TransferLensOrder = "TransferLensOrder";

    /// <summary>
    /// 盘点单
    /// </summary>
    public const string InventoryOrder = "InventoryOrder";

    /// <summary>
    /// 散镜订单
    /// </summary>
    public const string RetailOrder = "RetailOrder";

    /// <summary>
    /// 批量订单
    /// </summary>
    public const string BulkOrder = "BulkOrder";

    /// <summary>
    /// 外部出库单
    /// </summary>
    public const string ExternalLeaveStockOrder = "ExternalLeaveStockOrder";

    /// <summary>
    /// 出库单
    /// </summary>
    public const string LeaveStockOrder = "LeaveStockOrder";

    /// <summary>
    /// 入库单
    /// </summary>
    public const string EnterStockOrder = "EnterStockOrder";

    /// <summary>
    /// 申购单
    /// </summary>
    public const string PurchaseRequisitionOrder = "PurchaseRequisitionOrder";

    /// <summary>
    /// 出库批次
    /// </summary>
    public const string LeaveStockBatch = "LeaveStockBatch";

    /// <summary>
    /// 采购批次
    /// </summary>
    public const string PurchaseBatch = "PurchaseBatch";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        PurchaseOrder,
        TransferOrder,
        TransferLensOrder,
        InventoryOrder,
        RetailOrder,
        BulkOrder,
        ExternalLeaveStockOrder,
        LeaveStockOrder,
        EnterStockOrder,
        PurchaseRequisitionOrder,
        LeaveStockBatch,
        PurchaseBatch
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
