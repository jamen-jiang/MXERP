using System;
using System.Collections.Generic;
using System.Linq;
using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单
/// </summary>
public class PurchaseOrder : OrderAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public ProductTypeEnum ProductType { get; private set; }
    /// <summary>
    /// 关联类型
    /// </summary>
    public BusinessOrderTypeEnum RelationType { get; private set; }
    /// <summary>
    /// 仓位Id(可空)
    /// </summary>
    public long? PositionId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 供应商Id
    /// </summary>
    public long SupplierId { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 入库数量
    /// </summary>
    public double EnterStockQty { get; private set; }
    /// <summary>
    /// 金额
    /// </summary>
    public decimal Money { get; private set; }
    /// <summary>
    /// 支付方式(可空)
    /// </summary>
    public string? PayMode { get; private set; }
    /// <summary>
    /// 收货方式(可空)
    /// </summary>
    public string? DeliveryMode { get; private set; }
    /// <summary>
    /// 收货时间(可空)
    /// </summary>
    public DateTime? DeliveryOn { get; private set; }
    /// <summary>
    /// 收货人
    /// </summary>
    public string Deliveryer { get; private set; } = string.Empty;
    /// <summary>
    /// 收货人电话
    /// </summary>
    public string DeliveryerTelephone { get; private set; } = string.Empty;
    /// <summary>
    /// 收货地区
    /// </summary>
    public string DeliveryArea { get; private set; } = string.Empty;
    /// <summary>
    /// 收货地址
    /// </summary>
    public string DeliveryAddress { get; private set; } = string.Empty;
    /// <summary>
    /// 状态
    /// </summary>
    public PurchaseOrderStatusEnum Status { get; private set; }
    /// <summary>
    /// 用途(可空)
    /// </summary>
    public string? Purpose { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 审核人Id(可空)
    /// </summary>
    public long? CheckedBy { get; private set; }
    /// <summary>
    /// 审核人名称(可空)
    /// </summary>
    public string? CheckedByName { get; private set; }
    /// <summary>
    /// 审核时间(可空)
    /// </summary>
    public DateTime? CheckedOn { get; private set; }
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
    /// 采购单项列表
    /// </summary>
    private readonly List<PurchaseOrderItem> _purchaseOrderItems;

    /// <summary>
    /// 采购单项列表
    /// </summary>
    public IReadOnlyCollection<PurchaseOrderItem> PurchaseOrderItems
    {
        get { return _purchaseOrderItems.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected PurchaseOrder()
    {
        _purchaseOrderItems = new List<PurchaseOrderItem>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="positionId">仓位Id(可空)</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="payMode">支付方式(可空)</param>
    /// <param name="deliveryMode">收货方式(可空)</param>
    /// <param name="deliveryOn">收货时间(可空)</param>
    /// <param name="deliveryer">收货人</param>
    /// <param name="deliveryerTelephone">收货人电话</param>
    /// <param name="deliveryArea">收货地区</param>
    /// <param name="deliveryAddress">收货地址</param>
    /// <param name="remark">备注(可空)</param>
    internal PurchaseOrder(
        ProductTypeEnum productType,
        BusinessOrderTypeEnum relationType,
        long companyId,
        long? positionId,
        long supplierId,
        string? payMode,
        string? deliveryMode,
        DateTime? deliveryOn,
        string deliveryer,
        string deliveryerTelephone,
        string deliveryArea,
        string deliveryAddress,
        string? remark)
        : this()
    {
        ProductType = productType;
        RelationType = relationType;
        CompanyId = companyId;
        PositionId = positionId;
        SupplierId = supplierId;
        PayMode = payMode;
        DeliveryMode = deliveryMode;
        DeliveryOn = deliveryOn;
        Deliveryer = deliveryer;
        DeliveryerTelephone = deliveryerTelephone;
        DeliveryArea = deliveryArea;
        DeliveryAddress = deliveryAddress;
        Remark = remark;
        Status = PurchaseOrderStatusEnum.Pending;
        AddDomainEvent(new PurchaseOrderAddedEvent(this));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="positionId">仓位Id(可空)</param>
    /// <param name="payMode">支付方式(可空)</param>
    /// <param name="deliveryOn">收货时间(可空)</param>
    /// <param name="deliveryer">收货人</param>
    /// <param name="deliveryerTelephone">收货人电话</param>
    /// <param name="deliveryArea">收货地区</param>
    /// <param name="deliveryAddress">收货地址</param>
    /// <param name="remark">备注(可空)</param>
    public void Modify(
        long companyId,
        long? positionId,
        string? payMode,
        DateTime? deliveryOn,
        string deliveryer,
        string deliveryerTelephone,
        string deliveryArea,
        string deliveryAddress,
        string? remark)
    {
        if (Status != PurchaseOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("采购单" + Number + "当前状态不允许修改!");
        }

        CompanyId = companyId;
        PositionId = positionId;
        PayMode = payMode;
        DeliveryOn = deliveryOn;
        Deliveryer = deliveryer;
        DeliveryerTelephone = deliveryerTelephone;
        DeliveryArea = deliveryArea;
        DeliveryAddress = deliveryAddress;
        Remark = remark;
        Audit(AuditActionEnum.Modify);
    }

    /// <summary>
    /// 添加项
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">单价(可空)</param>
    /// <param name="taxRate">税率(可空)</param>
    /// <param name="discount">打折(可空)</param>
    /// <param name="skuAttributes">Sku属性值(可空)</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    internal void AddItem(
        BusinessOrderTypeEnum relationType,
        long relationId,
        long relationItemId,
        string relationNumber,
        long spuId,
        long skuId,
        long warehouseId,
        double qty,
        decimal? price,
        decimal? taxRate,
        double? discount,
        SortedDictionary<string, object?>? skuAttributes,
        SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        PurchaseOrderItem? item = _purchaseOrderItems.FirstOrDefault(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
        if (item != null)
        {
            item.AddQty(qty);
        }
        else
        {
            item = new PurchaseOrderItem(spuId, skuId, warehouseId, qty, price, taxRate, discount, skuAttributes, otherAttributes);
            _purchaseOrderItems.Add(item);
        }

        if (ProductType == ProductTypeEnum.EndProduct || ProductType == ProductTypeEnum.SemiProduct)
        {
            long distinctCount = _purchaseOrderItems.Select(x => x.SpuId + "_" + x.WarehouseId).Distinct().LongCount();
            if (distinctCount > 1)
            {
                throw new UserFriendlyException("成品或半成品采购订单不允许添加多个不同产品或不同仓库!");
            }
        }

        item.AddPurchaseOrderRelation(relationType, relationId, relationItemId, relationNumber, qty);
        UpdateQty();
        UpdateMoney();
    }

    /// <summary>
    /// 修改项
    /// </summary>
    /// <param name="itemId">采购单项Id</param>
    /// <param name="price">单价(可空)</param>
    /// <param name="taxRate">税率(可空)</param>
    /// <param name="discount">打折(可空)</param>
    public void ModifyItem(long itemId, decimal? price, decimal? taxRate, double? discount)
    {
        PurchaseOrderItem? item = _purchaseOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            throw new UserFriendlyException("采购单项不存在!");
        }

        item.Modify(price, taxRate, discount);
        UpdateMoney();
    }

    /// <summary>
    /// 添加采购项入库数量
    /// </summary>
    /// <param name="itemId">采购单项Id</param>
    /// <param name="qty">数量</param>
    public void AddEnterStockQty(long itemId, double qty)
    {
        if (qty == 0)
        {
            return;
        }

        PurchaseOrderItem? item = _purchaseOrderItems.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);
        if (item != null)
        {
            item.AddEnterStockQty(qty);
            UpdateEnterStockQty();
        }
    }

    /// <summary>
    /// 审核
    /// </summary>
    /// <param name="userId">审核人Id</param>
    /// <param name="name">审核人名称</param>
    public void Check(long userId, string name)
    {
        if (Status != PurchaseOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("采购单" + Number + "当前状态不允许审核!");
        }

        CheckedBy = userId;
        CheckedByName = name;
        CheckedOn = DateTime.Now;
        Status = PurchaseOrderStatusEnum.EnterStock;
        Audit(AuditActionEnum.Check);
    }

    /// <summary>
    /// 弃审
    /// </summary>
    /// <param name="remark">备注(可空)</param>
    public void UnCheck(string? remark)
    {
        if (Status != PurchaseOrderStatusEnum.EnterStock)
        {
            throw new UserFriendlyException("采购单" + Number + "当前状态不允许弃审!");
        }

        CheckedBy = null;
        CheckedByName = null;
        CheckedOn = null;
        Status = PurchaseOrderStatusEnum.Pending;
        Audit(AuditActionEnum.UnCheck, remark);
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注(可空)</param>
    public void Cancel(long userId, string name, string? remark)
    {
        if (Status != PurchaseOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("采购单" + Number + "当前状态不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = PurchaseOrderStatusEnum.Canceled;
        AddDomainEvent(new PurchaseOrderCanceledEvent(this));
        Audit(AuditActionEnum.Cancel, remark);
    }

    /// <summary>
    /// 欠数
    /// </summary>
    /// <returns>欠数</returns>
    public double GetOweQty()
    {
        return Qty - EnterStockQty;
    }

    /// <inheritdoc />
    public override SerialNumberTypeEnum GetSerialNumberType()
    {
        if (ProductType == ProductTypeEnum.SemiProduct)
        {
            return SerialNumberTypeEnum.SemiProductPurchaseOrder;
        }

        if (ProductType == ProductTypeEnum.EndProduct)
        {
            return SerialNumberTypeEnum.EndProductPurchaseOrder;
        }

        return SerialNumberTypeEnum.AccessoryPurchaseOrder;
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _purchaseOrderItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总金额
    /// </summary>
    private void UpdateMoney()
    {
        decimal total = decimal.Zero;
        foreach (PurchaseOrderItem item in _purchaseOrderItems)
        {
            if (!item.IsDeleted)
            {
                total += item.Money;
            }
        }

        Money = total;
    }

    /// <summary>
    /// 汇总入库数量, 并推进状态
    /// </summary>
    private void UpdateEnterStockQty()
    {
        EnterStockQty = _purchaseOrderItems.Where(x => !x.IsDeleted).Sum(x => x.EnterStockQty);
        if (EnterStockQty == 0)
        {
            Status = PurchaseOrderStatusEnum.EnterStock;
            return;
        }

        if (Qty - EnterStockQty > 0)
        {
            Status = PurchaseOrderStatusEnum.PartialEnterStock;
            return;
        }

        if (Qty - EnterStockQty == 0)
        {
            Status = PurchaseOrderStatusEnum.AllEnterStock;
            return;
        }

        Status = PurchaseOrderStatusEnum.OverEnterStock;
    }
}
