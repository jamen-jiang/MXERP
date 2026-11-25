using MXERP.Domain.Common.Contracts;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购订单项数据
/// </summary>
public class PurchaseOrderItemData : SkuInfoData
{
    /// <summary>
    /// 单价(可空)
    /// </summary>
    public decimal? Price { get; }
    /// <summary>
    /// 税率(可空)
    /// </summary>
    public decimal? TaxRate { get; }
    /// <summary>
    /// 打折(可空)
    /// </summary>
    public double? Discount { get; }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量(可空)</param>
    /// <param name="price">单价(可空)</param>
    /// <param name="taxRate">税率(可空)</param>
    /// <param name="discount">打折(可空)</param>
    public PurchaseOrderItemData(long skuId, long warehouseId, double? qty, decimal? price, decimal? taxRate, double? discount)
        : base(skuId, warehouseId, qty)
    {
        Price = price;
        TaxRate = taxRate;
        Discount = discount;
    }
}
