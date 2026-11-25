using System;
using System.Collections.Generic;
using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单详情
/// </summary>
public class InventoryOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 盘点单Id
    /// </summary>
    public long? InventoryOrderId { get; private set; }
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
    /// 盘点数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 盘盈数量
    /// </summary>
    public double ProfitQty { get; private set; }
    /// <summary>
    /// 盘亏数量
    /// </summary>
    public double LossQty { get; private set; }
    /// <summary>
    /// Sku属性值(可空)
    /// </summary>
    public SortedDictionary<string, object?>? SkuAttributes { get; private set; }
    /// <summary>
    /// 非关键属性值(可空)
    /// </summary>
    public SortedDictionary<string, object?>? OtherAttributes { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected InventoryOrderItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值(可空)</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    internal InventoryOrderItem(
        long spuId,
        long skuId,
        long warehouseId,
        double qty,
        SortedDictionary<string, object?>? skuAttributes,
        SortedDictionary<string, object?>? otherAttributes)
    {
        SpuId = spuId;
        SkuId = skuId;
        WarehouseId = warehouseId;
        Qty = qty;
        SkuAttributes = skuAttributes;
        OtherAttributes = otherAttributes;
    }

    /// <summary>
    /// 修改数量
    /// </summary>
    /// <param name="qty">数量</param>
    internal void Modify(double qty)
    {
        Qty = qty;
    }

    /// <summary>
    /// 盘点(计算盘盈、盘亏数量)
    /// </summary>
    /// <param name="stockQty">库存数量</param>
    internal void TaskStock(double stockQty)
    {
        double diff = Qty - stockQty;
        if (diff < 0)
        {
            ProfitQty = 0;
            LossQty = Math.Abs(diff);
        }
        else
        {
            ProfitQty = diff;
            LossQty = 0;
        }
    }
}
