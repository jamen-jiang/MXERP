namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项销售SKU行数据
/// </summary>
/// <param name="SalesSkuId">销售SkuId</param>
/// <param name="SkuAttributes">Sku关键属性值(可空)</param>
/// <param name="OtherAttributes">非关键属性值(可空)</param>
/// <param name="Qty">数量</param>
public sealed record BulkOrderItemSalesSkuData(long SalesSkuId, SortedDictionary<string, object?>? SkuAttributes, SortedDictionary<string, object?>? OtherAttributes, double Qty);
