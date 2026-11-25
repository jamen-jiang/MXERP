using System;
using System.Collections.Generic;
using System.Linq;
using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单
/// </summary>
public class InventoryOrder : OrderAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;
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
    /// 状态
    /// </summary>
    public string Status { get; private set; } = string.Empty;
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
    /// 盘点单项列表
    /// </summary>
    public virtual ICollection<InventoryOrderItem> InventoryOrderItems { get; private set; } = new List<InventoryOrderItem>();

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected InventoryOrder()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="remark">备注(可空)</param>
    internal InventoryOrder(string productType, string? remark)
    {
        ProductType = productType.Trim();
        Remark = remark;
        Status = InventoryOrderStatusEnum.Pending;
    }

    /// <summary>
    /// 获取盘点单项列表(只读)
    /// </summary>
    /// <returns>只读列表</returns>
    public IReadOnlyList<InventoryOrderItem> GetInventoryOrderItems()
    {
        return InventoryOrderItems.ToList().AsReadOnly();
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="remark">备注(可空)</param>
    public void Modify(string? remark)
    {
        if (Status != InventoryOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("盘点单" + Number + "当前状态不允许修改!");
        }

        Remark = remark;
        Audit(AuditActionEnum.Modify);
    }

    /// <summary>
    /// 审核
    /// </summary>
    /// <param name="userId">审核人Id</param>
    /// <param name="name">审核人名称</param>
    internal void Check(long userId, string name)
    {
        if (Status != InventoryOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("盘点单" + Number + "当前状态不允许审核!");
        }

        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        Status = InventoryOrderStatusEnum.Finished;
        AddDomainEvent(new InventoryOrderCheckedEvent(this));
        Audit(AuditActionEnum.Check);
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注(可空)</param>
    public void Cancel(long userId, string name, string? remark)
    {
        if (Status != InventoryOrderStatusEnum.Pending)
        {
            throw new UserFriendlyException("盘点单" + Number + "当前状态不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = InventoryOrderStatusEnum.Canceled;
        AddDomainEvent(new InventoryOrderCanceledEvent(this));
        Audit(AuditActionEnum.Cancel, remark);
    }

    /// <summary>
    /// 添加项
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值(可空)</param>
    /// <param name="otherAttributes">非关键属性值(可空)</param>
    public void AddItem(
        long spuId,
        long skuId,
        long warehouseId,
        double qty,
        SortedDictionary<string, object?>? skuAttributes,
        SortedDictionary<string, object?>? otherAttributes)
    {
        if (IsExistItem(skuId, warehouseId))
        {
            throw new UserFriendlyException("产品Sku " + skuId + "仓库 " + warehouseId + "已存在!");
        }

        InventoryOrderItems.Add(new InventoryOrderItem(spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes));
        UpdateQty();
    }

    /// <summary>
    /// 修改项
    /// </summary>
    /// <param name="itemId">盘点单项Id</param>
    /// <param name="qty">数量</param>
    public void ModifyItem(long itemId, double qty)
    {
        InventoryOrderItem? item = InventoryOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            throw new UserFriendlyException("盘点单" + Number + "盘点单项不存在!");
        }

        item.Modify(qty);
        UpdateQty();
    }

    /// <summary>
    /// 删除项(标记删除)
    /// </summary>
    /// <param name="itemId">盘点单项Id</param>
    public void DeleteItem(long itemId)
    {
        InventoryOrderItem? item = InventoryOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return;
        }

        item.Delete();
        UpdateQty();
    }

    /// <summary>
    /// 盘点
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="stockQty">库存数量</param>
    public void TaskStock(long skuId, long warehouseId, double stockQty)
    {
        InventoryOrderItem? item = InventoryOrderItems.FirstOrDefault(x => x.SkuId == skuId && x.WarehouseId == warehouseId);
        if (item is null)
        {
            return;
        }

        item.TaskStock(stockQty);
        UpdateProfitQty();
        UpdateLossQty();
    }

    /// <inheritdoc />
    public override string GetSerialNumberType()
    {
        if (ProductType == ProductTypeEnum.SemiProduct)
        {
            return SerialNumberTypeEnum.SemiProductInventoryOrder;
        }

        if (ProductType == ProductTypeEnum.EndProduct)
        {
            return SerialNumberTypeEnum.EndProductInventoryOrder;
        }

        return SerialNumberTypeEnum.AccessoryInventoryOrder;
    }

    /// <summary>
    /// 判断盘点单项是否存在(按 skuId + warehouseId, 未删除)
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistItem(long skuId, long warehouseId)
    {
        return InventoryOrderItems.Any(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = InventoryOrderItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总盘盈数量
    /// </summary>
    private void UpdateProfitQty()
    {
        ProfitQty = InventoryOrderItems.Where(x => !x.IsDeleted).Sum(x => x.ProfitQty);
    }

    /// <summary>
    /// 汇总盘亏数量
    /// </summary>
    private void UpdateLossQty()
    {
        LossQty = InventoryOrderItems.Where(x => !x.IsDeleted).Sum(x => x.LossQty);
    }
}
