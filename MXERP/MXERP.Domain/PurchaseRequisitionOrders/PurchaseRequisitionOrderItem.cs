using System;
using Snow.Domain.Entities;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单项
/// </summary>
public class PurchaseRequisitionOrderItem : DeletionAuditedEntity
{
    /// <summary>
    /// 申购单Id
    /// </summary>
    public long? PurchaseRequisitionOrderId { get; private set; }
    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }
    /// <summary>
    /// SkuId
    /// </summary>
    public long SkuId { get; private set; }
    /// <summary>
    /// 是否首次采购
    /// </summary>
    public bool IsFirst { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 抽仓数量
    /// </summary>
    public double DeductQty { get; private set; }
    /// <summary>
    /// 订购数量
    /// </summary>
    public double CustomizedQty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }
    /// <summary>
    /// 周用量
    /// </summary>
    public double WeeklyUsage { get; private set; }
    /// <summary>
    /// 现有库存
    /// </summary>
    public double CurrentQty { get; private set; }
    /// <summary>
    /// 用途
    /// </summary>
    public string Purpose { get; private set; } = string.Empty;
    /// <summary>
    /// 要求交期时间
    /// </summary>
    public DateTime MustDeliveryOn { get; private set; }
    /// <summary>
    /// 交货地址（可空）
    /// </summary>
    public string? DeliveryAddress { get; private set; }
    /// <summary>
    /// 预计交期时间（可空）
    /// </summary>
    public DateTime? DeliveryOn { get; private set; }
    /// <summary>
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected PurchaseRequisitionOrderItem()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="spuId">产品Id</param>
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
    internal PurchaseRequisitionOrderItem(
        long spuId,
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
        SpuId = spuId;
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
    /// 修改
    /// </summary>
    /// <param name="isFirst">是否首次采购</param>
    /// <param name="qty">数量</param>
    /// <param name="weeklyUsage">周用量</param>
    /// <param name="currentQty">现有库存</param>
    /// <param name="purpose">用途</param>
    /// <param name="mustDeliveryOn">要求交期时间</param>
    /// <param name="deliveryAddress">交货地址（可空）</param>
    /// <param name="deliveryOn">预计交期时间（可空）</param>
    /// <param name="remark">备注（可空）</param>
    internal void Modify(
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
    /// 欠数
    /// </summary>
    /// <returns>欠数</returns>
    public double GetOweQty()
    {
        return Qty - LeaveStockQty;
    }

    /// <summary>
    /// 未分配数量
    /// </summary>
    /// <returns>未分配数量</returns>
    public double GetNoAssignQty()
    {
        return Qty - DeductQty - CustomizedQty;
    }

    /// <summary>
    /// 是否已分配
    /// </summary>
    /// <returns>true 表示已分配</returns>
    public bool GetIsAssign()
    {
        return Math.Abs(DeductQty + CustomizedQty - Qty) < 1e-9;
    }

    /// <summary>
    /// 添加出库数量
    /// </summary>
    /// <param name="leaveStockQty">出库数量</param>
    internal void AddLeaveStockQty(double leaveStockQty)
    {
        LeaveStockQty += leaveStockQty;
    }

    /// <summary>
    /// 减去出库数量
    /// </summary>
    /// <param name="leaveStockQty">出库数量</param>
    internal void SubtractLeaveStockQty(double leaveStockQty)
    {
        LeaveStockQty -= leaveStockQty;
    }

    /// <summary>
    /// 更新抽仓数量
    /// </summary>
    /// <param name="deductQty">抽仓数量</param>
    internal void UpdateDeductQty(double deductQty)
    {
        DeductQty += deductQty;
    }

    /// <summary>
    /// 更新订购数量
    /// </summary>
    /// <param name="customizedQty">订购数量</param>
    internal void UpdateCustomizedQty(double customizedQty)
    {
        CustomizedQty += customizedQty;
    }
}
