namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号类型(对应 Java 的 SerialNumberTypeEnum, 以编码字符串存储)
/// </summary>
public static class SerialNumberTypeEnum
{
    /// <summary>
    /// 成品批量订单
    /// </summary>
    public const string EndProductBulkOrder = "EndProductBulkOrder";
    /// <summary>
    /// 成品盘点单
    /// </summary>
    public const string EndProductInventoryOrder = "EndProductInventoryOrder";
    /// <summary>
    /// 成品出库批次
    /// </summary>
    public const string EndProductLeaveStockBatch = "EndProductLeaveStockBatch";
    /// <summary>
    /// 成品采购批次
    /// </summary>
    public const string EndProductPurchaseBatch = "EndProductPurchaseBatch";
    /// <summary>
    /// 成品入库单
    /// </summary>
    public const string EndProductEnterStockOrder = "EndProductEnterStockOrder";
    /// <summary>
    /// 成品出库单
    /// </summary>
    public const string EndProductLeaveStockOrder = "EndProductLeaveStockOrder";
    /// <summary>
    /// 成品采购单
    /// </summary>
    public const string EndProductPurchaseOrder = "EndProductPurchaseOrder";
    /// <summary>
    /// 半成品批量订单
    /// </summary>
    public const string SemiProductBulkOrder = "SemiProductBulkOrder";
    /// <summary>
    /// 半成品盘点单
    /// </summary>
    public const string SemiProductInventoryOrder = "SemiProductInventoryOrder";
    /// <summary>
    /// 半成品出库批次
    /// </summary>
    public const string SemiProductLeaveStockBatch = "SemiProductLeaveStockBatch";
    /// <summary>
    /// 半成品采购批次
    /// </summary>
    public const string SemiProductPurchaseBatch = "SemiProductPurchaseBatch";
    /// <summary>
    /// 半成品入库单
    /// </summary>
    public const string SemiProductEnterStockOrder = "SemiProductEnterStockOrder";
    /// <summary>
    /// 半成品出库单
    /// </summary>
    public const string SemiProductLeaveStockOrder = "SemiProductLeaveStockOrder";
    /// <summary>
    /// 半成品采购单
    /// </summary>
    public const string SemiProductPurchaseOrder = "SemiProductPurchaseOrder";
    /// <summary>
    /// 辅料盘点单
    /// </summary>
    public const string AccessoryInventoryOrder = "AccessoryInventoryOrder";
    /// <summary>
    /// 辅料出库批次
    /// </summary>
    public const string AccessoryLeaveStockBatch = "AccessoryLeaveStockBatch";
    /// <summary>
    /// 辅料采购批次
    /// </summary>
    public const string AccessoryPurchaseBatch = "AccessoryPurchaseBatch";
    /// <summary>
    /// 辅料入库单
    /// </summary>
    public const string AccessoryEnterStockOrder = "AccessoryEnterStockOrder";
    /// <summary>
    /// 辅料出库单
    /// </summary>
    public const string AccessoryLeaveStockOrder = "AccessoryLeaveStockOrder";
    /// <summary>
    /// 辅料采购单
    /// </summary>
    public const string AccessoryPurchaseOrder = "AccessoryPurchaseOrder";
    /// <summary>
    /// 散镜订单
    /// </summary>
    public const string RetailOrder = "RetailOrder";
    /// <summary>
    /// 申购单
    /// </summary>
    public const string PurchaseRequisitionOrder = "PurchaseRequisitionOrder";
    /// <summary>
    /// 正常出仓单
    /// </summary>
    public const string NormalLeaveWarehouseOrder = "NormalLeaveWarehouseOrder";
    /// <summary>
    /// 借调出仓单
    /// </summary>
    public const string SecondLeaveWarehouseOrder = "SecondLeaveWarehouseOrder";
    /// <summary>
    /// 发票
    /// </summary>
    public const string Invoice = "Invoice";
    /// <summary>
    /// 三联发票
    /// </summary>
    public const string TripleInvoice = "TripleInvoice";
    /// <summary>
    /// 成品外部出库单
    /// </summary>
    public const string EndProductExternalLeaveStockOrder = "EndProductExternalLeaveStockOrder";
    /// <summary>
    /// 半成品外部出库单
    /// </summary>
    public const string SemiProductExternalLeaveStockOrder = "SemiProductExternalLeaveStockOrder";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        EndProductBulkOrder,
        EndProductInventoryOrder,
        EndProductLeaveStockBatch,
        EndProductPurchaseBatch,
        EndProductEnterStockOrder,
        EndProductLeaveStockOrder,
        EndProductPurchaseOrder,
        SemiProductBulkOrder,
        SemiProductInventoryOrder,
        SemiProductLeaveStockBatch,
        SemiProductPurchaseBatch,
        SemiProductEnterStockOrder,
        SemiProductLeaveStockOrder,
        SemiProductPurchaseOrder,
        AccessoryInventoryOrder,
        AccessoryLeaveStockBatch,
        AccessoryPurchaseBatch,
        AccessoryEnterStockOrder,
        AccessoryLeaveStockOrder,
        AccessoryPurchaseOrder,
        RetailOrder,
        PurchaseRequisitionOrder,
        NormalLeaveWarehouseOrder,
        SecondLeaveWarehouseOrder,
        Invoice,
        TripleInvoice,
        EndProductExternalLeaveStockOrder,
        SemiProductExternalLeaveStockOrder
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
