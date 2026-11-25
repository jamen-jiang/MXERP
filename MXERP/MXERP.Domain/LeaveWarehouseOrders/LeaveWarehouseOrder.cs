using MXERP.Domain.Common.Enums;

using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.SerialNumbers;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单
/// </summary>
public class LeaveWarehouseOrder : OrderAggregateRoot
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveWarehouseOrder()
    {
        Sender = string.Empty;
        Deliveryer = string.Empty;
        _leaveWarehouseOrderItems = new List<LeaveWarehouseOrderItem>();
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="type">类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="sendCompanyId">发货公司Id</param>
    /// <param name="sender">发货人</param>
    /// <param name="senderTelephone">发货人电话(可空)</param>
    /// <param name="sendArea">发货地区(可空)</param>
    /// <param name="sendAddress">发货地址(可空)</param>
    /// <param name="deliveryCompanyId">收货公司Id</param>
    /// <param name="deliveryer">收货人</param>
    /// <param name="deliveryerTelephone">收货人电话(可空)</param>
    /// <param name="deliveryArea">收货地区(可空)</param>
    /// <param name="deliveryAddress">收货地址(可空)</param>
    internal LeaveWarehouseOrder(ProductTypeEnum productType, LeaveWarehouseOrderTypeEnum type, long companyId, long sendCompanyId, string sender, string? senderTelephone, string? sendArea, string? sendAddress, long deliveryCompanyId, string deliveryer, string? deliveryerTelephone, string? deliveryArea, string? deliveryAddress)
        : this()
    {
        ProductType = productType;
        Type = type;
        CompanyId = companyId;
        SendCompanyId = sendCompanyId;
        Sender = sender;
        SenderTelephone = senderTelephone;
        SendArea = sendArea;
        SendAddress = sendAddress;
        DeliveryCompanyId = deliveryCompanyId;
        Deliveryer = deliveryer;
        DeliveryerTelephone = deliveryerTelephone;
        DeliveryArea = deliveryArea;
        DeliveryAddress = deliveryAddress;
    }

    /// <summary>
    /// 产品类型
    /// </summary>
    public ProductTypeEnum ProductType { get; private set; }

    /// <summary>
    /// 类型
    /// </summary>
    public LeaveWarehouseOrderTypeEnum Type { get; private set; }

    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }

    /// <summary>
    /// 发货公司Id
    /// </summary>
    public long SendCompanyId { get; private set; }

    /// <summary>
    /// 发货人
    /// </summary>
    public string Sender { get; private set; }

    /// <summary>
    /// 发货人电话(可空)
    /// </summary>
    public string? SenderTelephone { get; private set; }

    /// <summary>
    /// 发货地区(可空)
    /// </summary>
    public string? SendArea { get; private set; }

    /// <summary>
    /// 发货地址(可空)
    /// </summary>
    public string? SendAddress { get; private set; }

    /// <summary>
    /// 收货公司Id
    /// </summary>
    public long DeliveryCompanyId { get; private set; }

    /// <summary>
    /// 收货人
    /// </summary>
    public string Deliveryer { get; private set; }

    /// <summary>
    /// 收货人电话(可空)
    /// </summary>
    public string? DeliveryerTelephone { get; private set; }

    /// <summary>
    /// 收货地区(可空)
    /// </summary>
    public string? DeliveryArea { get; private set; }

    /// <summary>
    /// 收货地址(可空)
    /// </summary>
    public string? DeliveryAddress { get; private set; }

    /// <summary>
    /// 出仓数量
    /// </summary>
    public double Qty { get; private set; }

    /// <summary>
    /// 发票金额
    /// </summary>
    public decimal Money { get; private set; }

    /// <summary>
    /// 是否打印
    /// </summary>
    public bool IsPrinted { get; private set; }

    /// <summary>
    /// 打印人Id(可空)
    /// </summary>
    public long? PrintedBy { get; private set; }

    /// <summary>
    /// 打印人(可空)
    /// </summary>
    public string? PrintedByName { get; private set; }

    /// <summary>
    /// 打印时间(可空)
    /// </summary>
    public DateTime? PrintedOn { get; private set; }

    /// <summary>
    /// 出仓单详情列表
    /// </summary>
    private readonly List<LeaveWarehouseOrderItem> _leaveWarehouseOrderItems;

    /// <summary>
    /// 出仓单详情列表
    /// </summary>
    public IReadOnlyCollection<LeaveWarehouseOrderItem> LeaveWarehouseOrderItems
    {
        get { return _leaveWarehouseOrderItems.AsReadOnly(); }
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="sendCompanyId">发货公司Id</param>
    /// <param name="sender">发货人</param>
    /// <param name="senderTelephone">发货人电话(可空)</param>
    /// <param name="sendArea">发货地区(可空)</param>
    /// <param name="sendAddress">发货地址(可空)</param>
    /// <param name="deliveryCompanyId">收货公司Id</param>
    /// <param name="deliveryer">收货人</param>
    /// <param name="deliveryerTelephone">收货人电话(可空)</param>
    /// <param name="deliveryArea">收货地区(可空)</param>
    /// <param name="deliveryAddress">收货地址(可空)</param>
    public void Modify(long sendCompanyId, string sender, string? senderTelephone, string? sendArea, string? sendAddress, long deliveryCompanyId, string deliveryer, string? deliveryerTelephone, string? deliveryArea, string? deliveryAddress)
    {
        SendCompanyId = sendCompanyId;
        Sender = sender;
        SenderTelephone = senderTelephone;
        SendArea = sendArea;
        SendAddress = sendAddress;
        DeliveryCompanyId = deliveryCompanyId;
        Deliveryer = deliveryer;
        DeliveryerTelephone = deliveryerTelephone;
        DeliveryArea = deliveryArea;
        DeliveryAddress = deliveryAddress;
    }

    /// <summary>
    /// 添加项
    /// </summary>
    /// <param name="leaveStockBatchId">出库批次Id</param>
    /// <param name="leaveStockBatchNumber">出库批次号</param>
    /// <param name="salesSpuId">销售产品Id(可空)</param>
    /// <param name="salesSpuCode">销售产品Code(可空)</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="remark">备注(可空)</param>
    internal void AddItem(long leaveStockBatchId, string leaveStockBatchNumber, long? salesSpuId, string? salesSpuCode, long spuId, double qty, decimal price, string? remark)
    {
        LeaveWarehouseOrderItem? item = _leaveWarehouseOrderItems
            .FirstOrDefault(x => x.LeaveStockBatchId == leaveStockBatchId && x.SalesSpuId == salesSpuId && x.SpuId == spuId && !x.IsDeleted);

        if (item is not null)
        {
            if (item.Qty.Equals(qty))
            {
                item.Modify(remark);
                return;
            }

            DeleteItem(item.Id);
        }

        item = new LeaveWarehouseOrderItem(leaveStockBatchId, leaveStockBatchNumber, salesSpuId, salesSpuCode, spuId, qty, price, remark);
        _leaveWarehouseOrderItems.Add(item);
        AddDomainEvent(new LeaveWarehouseOrderItemAddedEvent(this, item.LeaveStockBatchId, item.SalesSpuId, item.SpuId, item.Qty));
        UpdateQty();
        UpdateMoney();
    }

    /// <summary>
    /// 删除项(标记删除)
    /// </summary>
    /// <param name="itemId">出仓单项Id</param>
    public void DeleteItem(long itemId)
    {
        LeaveWarehouseOrderItem? item = _leaveWarehouseOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return;
        }

        item.Delete();
        AddDomainEvent(new LeaveWarehouseOrderItemDeletedEvent(this, item.LeaveStockBatchId, item.SalesSpuId, item.SpuId, item.Qty));
        UpdateQty();
        UpdateMoney();
    }

    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="userId">打印人Id</param>
    /// <param name="name">打印人名称</param>
    public void Print(long userId, string name)
    {
        IsPrinted = true;
        PrintedBy = userId;
        PrintedByName = name;
        PrintedOn = DateTime.Now;
    }

    /// <summary>
    /// 删除(同时派发所有出仓单项的移除事件)
    /// </summary>
    public override void Delete()
    {
        base.Delete();
        foreach (LeaveWarehouseOrderItem item in _leaveWarehouseOrderItems)
        {
            AddDomainEvent(new LeaveWarehouseOrderItemDeletedEvent(this, item.LeaveStockBatchId, item.SalesSpuId, item.SpuId, item.Qty));
        }
    }

    /// <summary>
    /// 修改创建信息
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="userName">用户名</param>
    /// <param name="dateTime">时间(可空)</param>
    public void ModifyCreateInfo(long userId, string userName, DateTime? dateTime)
    {
        Create(userId, userName, dateTime);
        AddDomainEvent(new LeaveWarehouseOrderCreateInfoModifiedEvent(this));
    }

    /// <summary>
    /// 序列号类型
    /// </summary>
    /// <returns>序列号类型</returns>
    public override SerialNumberTypeEnum GetSerialNumberType()
    {
        if (Type == LeaveWarehouseOrderTypeEnum.Second)
        {
            return SerialNumberTypeEnum.SecondLeaveWarehouseOrder;
        }

        return SerialNumberTypeEnum.NormalLeaveWarehouseOrder;
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _leaveWarehouseOrderItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总金额
    /// </summary>
    private void UpdateMoney()
    {
        decimal total = 0;
        foreach (LeaveWarehouseOrderItem item in _leaveWarehouseOrderItems)
        {
            if (!item.IsDeleted)
            {
                total += (decimal)item.Qty * item.Price;
            }
        }

        Money = total;
    }
}