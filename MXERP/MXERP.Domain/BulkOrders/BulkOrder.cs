using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.BulkOrders;

/// <summary>
/// 批量订单
/// </summary>
public class BulkOrder : OrderAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public ProductTypeEnum ProductType { get; private set; }
    /// <summary>
    /// 客户单号
    /// </summary>
    public string CustomerOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 公司订单编号(可空)
    /// </summary>
    public string? OrderRef { get; private set; }
    /// <summary>
    /// 客户PO号(可空)
    /// </summary>
    public string? CustomerPONumber { get; private set; }
    /// <summary>
    /// 客户流水号(可空)
    /// </summary>
    public string? CustomerSerialNumber { get; private set; }
    /// <summary>
    /// 门店Id
    /// </summary>
    public long StoreId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 运输方式(可空)
    /// </summary>
    public string? TransportWay { get; private set; }
    /// <summary>
    /// 预出货时间(可空)
    /// </summary>
    public DateTime? DeliveryOn { get; private set; }
    /// <summary>
    /// 状态
    /// </summary>
    public BulkOrderStatusEnum Status { get; private set; }
    /// <summary>
    /// 通过人Id(可空)
    /// </summary>
    public long? PassedBy { get; private set; }
    /// <summary>
    /// 通过人名称(可空)
    /// </summary>
    public string? PassedByName { get; private set; }
    /// <summary>
    /// 通过时间(可空)
    /// </summary>
    public DateTime? PassedOn { get; private set; }
    /// <summary>
    /// 完成人Id(可空)
    /// </summary>
    public long? FinishedBy { get; private set; }
    /// <summary>
    /// 完成人名称(可空)
    /// </summary>
    public string? FinishedByName { get; private set; }
    /// <summary>
    /// 完成时间(可空)
    /// </summary>
    public DateTime? FinishedOn { get; private set; }
    /// <summary>
    /// 取消人Id(可空)
    /// </summary>
    public long? CanceledBy { get; private set; }
    /// <summary>
    /// 取消人名称(可空)
    /// </summary>
    public string? CanceledByName { get; private set; }
    /// <summary>
    /// 取消时间(可空)
    /// </summary>
    public DateTime? CanceledOn { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }
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
    /// 构造器(供 EF 使用)
    /// </summary>
    protected BulkOrder()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="orderRef">公司订单编号(可空)</param>
    /// <param name="customerSerialNumber">客户流水号(可空)</param>
    /// <param name="transportWay">运输方式(可空)</param>
    /// <param name="deliveryOn">预出货时间(可空)</param>
    /// <param name="qty">数量</param>
    /// <param name="remark">备注(可空)</param>
    internal BulkOrder(ProductTypeEnum productType, long storeId, long companyId, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark)
    {
        string? trimmedCustomerOrderNumber = TrimToNull(customerOrderNumber);

        ProductType = productType;
        StoreId = storeId;
        CompanyId = companyId;
        CustomerOrderNumber = trimmedCustomerOrderNumber ?? string.Empty;
        OrderRef = TrimToNull(orderRef);
        CustomerPONumber = TrimToNull(customerPONumber);
        CustomerSerialNumber = TrimToNull(customerSerialNumber);
        TransportWay = transportWay;
        DeliveryOn = deliveryOn;
        Qty = qty;
        Remark = remark;
        Status = BulkOrderStatusEnum.Pending;
        AddDomainEvent(new BulkOrderAddedEvent(this));
    }

    /// <summary>
    /// 修改(完整信息)
    /// </summary>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="orderRef">公司订单编号(可空)</param>
    /// <param name="customerSerialNumber">客户流水号(可空)</param>
    /// <param name="transportWay">运输方式(可空)</param>
    /// <param name="deliveryOn">预出货时间(可空)</param>
    /// <param name="qty">数量</param>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string? transportWay, DateTime? deliveryOn, double qty, string? remark)
    {
        if (Status != BulkOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("批量订单" + Number + "当前状态不允许修改!");
        }

        if (CustomizedQty > 0 || DeductQty > 0)
        {
            throw new UserFriendlyException("批量订单" + Number + "已分配不允许修改!");
        }

        string? trimmedCustomerOrderNumber = TrimToNull(customerOrderNumber);
        if (trimmedCustomerOrderNumber is null)
        {
            throw new UserFriendlyException("客户单号不能为空");
        }

        CustomerOrderNumber = trimmedCustomerOrderNumber;
        OrderRef = TrimToNull(orderRef);
        CustomerPONumber = TrimToNull(customerPONumber);
        CustomerSerialNumber = TrimToNull(customerSerialNumber);
        TransportWay = transportWay;
        DeliveryOn = deliveryOn;
        Qty = qty;
        Remark = remark;
        Audit(AuditActionEnum.Modify);
    }

    /// <summary>
    /// 修改(仅运输信息)
    /// </summary>
    /// <param name="transportWay">运输方式(可空)</param>
    /// <param name="deliveryOn">预出货时间(可空)</param>
    public void Modify(string? transportWay, DateTime? deliveryOn)
    {
        if (Status != BulkOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("批量订单" + Number + "当前状态不允许修改!");
        }

        TransportWay = transportWay;
        DeliveryOn = deliveryOn;
        Audit(AuditActionEnum.Modify);
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
    /// 更新出库数量
    /// </summary>
    /// <param name="qty">数量</param>
    public void UpdateLeaveStockQty(double qty)
    {
        LeaveStockQty += qty;
        UpdateStatusByLeaveStockQty();
    }

    /// <summary>
    /// 更新抽仓数量
    /// </summary>
    /// <param name="qty">数量</param>
    public void UpdateDeductQty(double qty)
    {
        DeductQty += qty;
    }

    /// <summary>
    /// 更新订购数量
    /// </summary>
    /// <param name="qty">数量</param>
    public void UpdateCustomizedQty(double qty)
    {
        CustomizedQty += qty;
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注(可空)</param>
    internal void Cancel(long userId, string name, string? remark)
    {
        if (Status != BulkOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("批量订单" + Number + "当前状态不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = BulkOrderStatusEnum.Canceled;
        AddDomainEvent(new BulkOrderCanceledEvent(this));
        Audit(AuditActionEnum.Cancel, remark);
    }

    /// <summary>
    /// 完成
    /// </summary>
    /// <param name="userId">完成人Id</param>
    /// <param name="name">完成人名称</param>
    public void Finish(long userId, string name)
    {
        if (Status != BulkOrderStatusEnum.AllDelivered && Status != BulkOrderStatusEnum.OverDelivered)
        {
            throw new UserFriendlyException("批量订单" + Number + "当前状态不允许完成!");
        }

        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        Status = BulkOrderStatusEnum.Finished;
        Audit(AuditActionEnum.Finish);
    }

    /// <summary>
    /// 获取流水号类型
    /// </summary>
    /// <returns>流水号类型</returns>
    public override SerialNumberTypeEnum GetSerialNumberType()
    {
        if (ProductType == ProductTypeEnum.SemiProduct)
        {
            return SerialNumberTypeEnum.SemiProductBulkOrder;
        }

        return SerialNumberTypeEnum.EndProductBulkOrder;
    }


    /// <summary>
    /// 按出库数量推进状态
    /// </summary>
    private void UpdateStatusByLeaveStockQty()
    {
        if (LeaveStockQty == 0)
        {
            Status = BulkOrderStatusEnum.Pending;
        }
        else if (Qty - LeaveStockQty > 0)
        {
            Status = BulkOrderStatusEnum.PartialDelivered;
        }
        else if (Qty - LeaveStockQty == 0)
        {
            Status = BulkOrderStatusEnum.AllDelivered;
        }
        else
        {
            Status = BulkOrderStatusEnum.OverDelivered;
        }
    }

    /// <summary>
    /// trim 后为空或 null 则返回 null
    /// </summary>
    /// <param name="value">原始值(可空)</param>
    /// <returns>trim 后的值, 空串返回 null</returns>
    private static string? TrimToNull(string? value)
    {
        if (value is null)
        {
            return null;
        }

        string trimmed = value.Trim();
        if (trimmed.Length == 0)
        {
            return null;
        }

        return trimmed;
    }
}
