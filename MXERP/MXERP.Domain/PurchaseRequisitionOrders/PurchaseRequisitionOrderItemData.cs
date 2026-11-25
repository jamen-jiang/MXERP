using System;
using System.Collections.Generic;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单项数据
/// </summary>
public class PurchaseRequisitionOrderItemData
{
    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="isFirst">是否首次采购</param>
    /// <param name="qty">数量</param>
    /// <param name="weeklyUsage">周用量</param>
    /// <param name="currentQty">现有库存</param>
    /// <param name="purpose">用途</param>
    /// <param name="mustDeliveryOn">要求交期时间</param>
    /// <param name="deliveryAddress">交货地址（可空）</param>
    /// <param name="deliveryOn">预计交期时间（可空）</param>
    /// <param name="remark">备注（可空）</param>
    public PurchaseRequisitionOrderItemData(
        long skuId,
        bool isFirst,
        double qty,
        double weeklyUsage,
        double currentQty,
        string purpose,
        DateTime mustDeliveryOn,
        string? deliveryAddress,
        DateTime? deliveryOn,
        string? remark)
    {
        SkuId = skuId;
        IsFirst = isFirst;
        Qty = qty;
        WeeklyUsage = weeklyUsage;
        CurrentQty = currentQty;
        Purpose = purpose;
        MustDeliveryOn = mustDeliveryOn;
        DeliveryAddress = deliveryAddress;
        DeliveryOn = deliveryOn;
        Remark = remark;
    }

    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; set; }
    /// <summary>
    /// 产品SkuId
    /// </summary>
    public long SkuId { get; }
    /// <summary>
    /// 是否首次采购
    /// </summary>
    public bool IsFirst { get; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; }
    /// <summary>
    /// 周用量
    /// </summary>
    public double WeeklyUsage { get; }
    /// <summary>
    /// 现有库存
    /// </summary>
    public double CurrentQty { get; }
    /// <summary>
    /// 用途
    /// </summary>
    public string Purpose { get; }
    /// <summary>
    /// 要求交期时间
    /// </summary>
    public DateTime MustDeliveryOn { get; }
    /// <summary>
    /// 交货地址（可空）
    /// </summary>
    public string? DeliveryAddress { get; }
    /// <summary>
    /// 预计交期时间（可空）
    /// </summary>
    public DateTime? DeliveryOn { get; }
    /// <summary>
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; }
}
