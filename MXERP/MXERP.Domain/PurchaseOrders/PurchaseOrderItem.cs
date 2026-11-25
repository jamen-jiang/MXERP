using MXERP.Domain.Common.Enums;
using System.Collections.Generic;
using System.Linq;
using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单项
/// </summary>
public class PurchaseOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 采购单Id
    /// </summary>
    public long? PurchaseOrderId { get; private set; }
    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }
    /// <summary>
    /// 产品SkuId
    /// </summary>
    public long SkuId { get; private set; }
    /// <summary>
    /// 仓库Id
    /// </summary>
    public long WarehouseId { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 入库数量
    /// </summary>
    public double EnterStockQty { get; private set; }
    /// <summary>
    /// 单价(可空)
    /// </summary>
    public decimal? Price { get; private set; }
    /// <summary>
    /// 税率(可空)
    /// </summary>
    public decimal? TaxRate { get; private set; }
    /// <summary>
    /// 打折(可空)
    /// </summary>
    public double? Discount { get; private set; }
    /// <summary>
    /// 金额
    /// </summary>
    public decimal Money { get; private set; }
    /// <summary>
    /// Sku属性值(可空)
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }
    /// <summary>
    /// 非关键属性值(可空)
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }
    /// <summary>
    /// 采购单项关联列表
    /// </summary>
    private readonly List<PurchaseOrderRelation> _purchaseOrderRelations;

    /// <summary>
    /// 采购单项关联列表
    /// </summary>
    public IReadOnlyCollection<PurchaseOrderRelation> PurchaseOrderRelations
    {
        get { return _purchaseOrderRelations.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器
    /// </summary>
    protected PurchaseOrderItem()
    {
        _purchaseOrderRelations = new List<PurchaseOrderRelation>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">单价(可空)</param>
    /// <param name="taxRate">税率(可空)</param>
    /// <param name="discount">打折(可空)</param>
    /// <param name="skuAttributes">Sku属性值(可空)</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    internal PurchaseOrderItem(
        long spuId,
        long skuId,
        long warehouseId,
        double qty,
        decimal? price,
        decimal? taxRate,
        double? discount,
        SortedDictionary<string, object?>? skuAttributes,
        SortedDictionary<string, object?>? otherAttributes)
        : this()
    {
        SpuId = spuId;
        SkuId = skuId;
        WarehouseId = warehouseId;
        Qty = qty;
        Price = price;
        TaxRate = taxRate;
        Discount = discount;
        SkuAttributes = skuAttributes;
        OtherAttributes = otherAttributes;
        UpdateMoney();
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="price">单价(可空)</param>
    /// <param name="taxRate">税率(可空)</param>
    /// <param name="discount">打折(可空)</param>
    internal void Modify(decimal? price, decimal? taxRate, double? discount)
    {
        Price = price;
        TaxRate = taxRate;
        Discount = discount;
        UpdateMoney();
    }

    /// <summary>
    /// 添加数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void AddQty(double qty)
    {
        Qty += qty;
    }

    /// <summary>
    /// 添加入库数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void AddEnterStockQty(double qty)
    {
        EnterStockQty += qty;
        if (EnterStockQty < 0)
        {
            throw new UserFriendlyException("入库数量不允许小于0!");
        }
    }

    /// <summary>
    /// 添加采购单关联
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="qty">数量</param>
    internal void AddPurchaseOrderRelation(BusinessOrderTypeEnum relationType, long relationId, long relationItemId, string relationNumber, double qty)
    {
        _purchaseOrderRelations.Add(new PurchaseOrderRelation(relationType, relationId, relationItemId, relationNumber, qty));
    }

    /// <summary>
    /// 欠数
    /// </summary>
    /// <returns>欠数</returns>
    public double GetOweQty()
    {
        return Qty - EnterStockQty;
    }

    /// <summary>
    /// 计算金额
    /// </summary>
    private void UpdateMoney()
    {
        decimal qtyDecimal = (decimal)Qty;
        decimal priceValue = Price ?? decimal.Zero;
        decimal taxRateValue = TaxRate ?? decimal.Zero;
        decimal discountValue = decimal.One;
        if (Discount != null && Discount > 0)
        {
            discountValue = (decimal)Discount.Value;
        }

        Money = qtyDecimal * priceValue * (taxRateValue + decimal.One) * discountValue;
    }
}
