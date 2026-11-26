using System;
using System.Collections.Generic;
using System.Linq;
using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.SerialNumbers;
using MXERP.Domain.Works;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单
/// </summary>
public class PurchaseRequisitionOrder : OrderAggregateRoot
{
    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
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
    /// 状态
    /// </summary>
    public string Status { get; private set; } = string.Empty;
    /// <summary>
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 提交人Id（可空）
    /// </summary>
    public long? CommittedBy { get; private set; }
    /// <summary>
    /// 提交人名称（可空）
    /// </summary>
    public string? CommittedByName { get; private set; }
    /// <summary>
    /// 提交时间（可空）
    /// </summary>
    public DateTime? CommittedOn { get; private set; }
    /// <summary>
    /// 审核人Id（可空）
    /// </summary>
    public long? CheckedBy { get; private set; }
    /// <summary>
    /// 审核人名称（可空）
    /// </summary>
    public string? CheckedByName { get; private set; }
    /// <summary>
    /// 审核时间（可空）
    /// </summary>
    public DateTime? CheckedOn { get; private set; }
    /// <summary>
    /// 完成人Id（可空）
    /// </summary>
    public long? FinishedBy { get; private set; }
    /// <summary>
    /// 完成人（可空）
    /// </summary>
    public string? FinishedByName { get; private set; }
    /// <summary>
    /// 完成时间（可空）
    /// </summary>
    public DateTime? FinishedOn { get; private set; }
    /// <summary>
    /// 取消人Id（可空）
    /// </summary>
    public long? CanceledBy { get; private set; }
    /// <summary>
    /// 取消人名称（可空）
    /// </summary>
    public string? CanceledByName { get; private set; }
    /// <summary>
    /// 取消时间（可空）
    /// </summary>
    public DateTime? CanceledOn { get; private set; }
    /// <summary>
    /// 申购单项列表
    /// </summary>
    private readonly List<PurchaseRequisitionOrderItem> _purchaseRequisitionOrderItems;

    /// <summary>
    /// 申购单项列表
    /// </summary>
    public IReadOnlyCollection<PurchaseRequisitionOrderItem> PurchaseRequisitionOrderItems
    {
        get { return _purchaseRequisitionOrderItems.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected PurchaseRequisitionOrder()
    {
        _purchaseRequisitionOrderItems = new List<PurchaseRequisitionOrderItem>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="remark">备注（可空）</param>
    internal PurchaseRequisitionOrder(long departmentId, long companyId, string? remark)
        : this()
    {
        DepartmentId = departmentId;
        CompanyId = companyId;
        Remark = remark;
        Status = nameof(PurchaseRequisitionOrderStatusEnum.Pending);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="remark">备注（可空）</param>
    public void Modify(long departmentId, long companyId, string? remark)
    {
        if (Status != nameof(PurchaseRequisitionOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("申购单" + Number + "当前状态不允许修改!");
        }

        DepartmentId = departmentId;
        CompanyId = companyId;
        Remark = remark;
        UpdateQty();
        Audit(nameof(AuditActionEnum.Modify));
    }

    /// <summary>
    /// 添加项
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
    internal void AddItem(
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
        if (IsExistItem(skuId))
        {
            throw new UserFriendlyException("产品Sku " + skuId + "已存在!");
        }

        _purchaseRequisitionOrderItems.Add(new PurchaseRequisitionOrderItem(spuId, skuId, isFirst, qty, weeklyUsage, currentQty, purpose, mustDeliveryOn, deliveryAddress, deliveryOn, remark));
        UpdateQty();
    }

    /// <summary>
    /// 修改项
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    /// <param name="isFirst">是否首次采购</param>
    /// <param name="qty">数量</param>
    /// <param name="weeklyUsage">周用量</param>
    /// <param name="currentQty">现有库存</param>
    /// <param name="purpose">用途</param>
    /// <param name="mustDeliveryOn">要求交期时间</param>
    /// <param name="deliveryAddress">交货地址（可空）</param>
    /// <param name="deliveryOn">预计交期时间（可空）</param>
    /// <param name="remark">备注（可空）</param>
    internal void ModifyItem(
        long itemId,
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
        PurchaseRequisitionOrderItem? item = _purchaseRequisitionOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            throw new UserFriendlyException("申购单项不存在!");
        }

        item.Modify(isFirst, qty, weeklyUsage, currentQty, purpose, mustDeliveryOn, deliveryAddress, deliveryOn, remark);
        UpdateQty();
    }

    /// <summary>
    /// 删除项（标记删除）
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    internal void DeleteItem(long itemId)
    {
        PurchaseRequisitionOrderItem? item = _purchaseRequisitionOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return;
        }

        item.Delete();
        UpdateQty();
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <param name="userId">提交人Id</param>
    /// <param name="name">提交人名称</param>
    public void Commit(long userId, string name)
    {
        if (Status != nameof(PurchaseRequisitionOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("申购单" + Number + "当前状态不允许提交!");
        }

        CommittedBy = userId;
        CommittedByName = name;
        CommittedOn = DateTime.Now;
        Status = nameof(PurchaseRequisitionOrderStatusEnum.PendingApproval);
        AddDomainEvent(new PurchaseRequisitionOrderCommittedEvent(this));
        Audit(nameof(AuditActionEnum.Commit));
    }

    /// <summary>
    /// 弃提
    /// </summary>
    internal void UnCommit()
    {
        CommittedBy = null;
        CommittedByName = null;
        CommittedOn = null;
        Status = nameof(PurchaseRequisitionOrderStatusEnum.Pending);
        Audit(nameof(AuditActionEnum.UnCommit));
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注（可空）</param>
    public void Cancel(long userId, string name, string? remark)
    {
        if (Status != nameof(PurchaseRequisitionOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("申购单" + Number + "当前状态不允许取消!");
        }

        Status = nameof(PurchaseRequisitionOrderStatusEnum.Canceled);
        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Audit(nameof(AuditActionEnum.Cancel), remark);
    }

    /// <summary>
    /// 采购审核
    /// </summary>
    /// <param name="userId">审核人Id</param>
    /// <param name="name">审核人名称</param>
    public void Check(long userId, string name)
    {
        if (Status != nameof(PurchaseRequisitionOrderStatusEnum.PendingPurchaseCheck))
        {
            throw new UserFriendlyException("申购单" + Number + "当前状态不允许采购审核!");
        }

        bool hasUnassigned = _purchaseRequisitionOrderItems.Any(x => !x.GetIsAssign() && !x.IsDeleted);
        if (hasUnassigned)
        {
            throw new UserFriendlyException("申购单" + Number + "必须所有产品已分配!");
        }

        Status = nameof(PurchaseRequisitionOrderStatusEnum.Delivery);
        CheckedBy = userId;
        CheckedByName = name;
        CheckedOn = DateTime.Now;
        AddDomainEvent(new PurchaseRequisitionOrderPurchaseCheckedEvent(this));
        Audit(nameof(AuditActionEnum.Check));
    }

    /// <summary>
    /// 添加项出库数量
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    /// <param name="qty">数量</param>
    public void AddItemLeaveStockQty(long itemId, double qty)
    {
        if (qty > 0)
        {
            PurchaseRequisitionOrderItem? item = FindItem(itemId);
            if (item is null)
            {
                return;
            }

            item.AddLeaveStockQty(qty);
            UpdateLeaveStockQty();
        }
    }

    /// <summary>
    /// 减去项出库数量
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    /// <param name="qty">数量</param>
    public void SubtractItemLeaveStockQty(long itemId, double qty)
    {
        if (qty > 0)
        {
            PurchaseRequisitionOrderItem? item = FindItem(itemId);
            if (item is null)
            {
                return;
            }

            item.SubtractLeaveStockQty(qty);
            UpdateLeaveStockQty();
        }
    }

    /// <summary>
    /// 更新项抽仓数量
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    /// <param name="qty">数量</param>
    public void UpdateItemDeductQty(long itemId, double qty)
    {
        if (qty > 0)
        {
            PurchaseRequisitionOrderItem? item = FindItem(itemId);
            if (item is null)
            {
                return;
            }

            item.UpdateDeductQty(qty);
            UpdateDeductQty();
        }
    }

    /// <summary>
    /// 更新项订购数量
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    /// <param name="qty">数量</param>
    public void UpdateItemCustomizedQty(long itemId, double qty)
    {
        if (qty > 0)
        {
            PurchaseRequisitionOrderItem? item = FindItem(itemId);
            if (item is null)
            {
                return;
            }

            item.UpdateCustomizedQty(qty);
            UpdateCustomizedQty();
        }
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

    /// <inheritdoc />
    public override string GetSerialNumberType()
    {
        return nameof(SerialNumberTypeEnum.PurchaseRequisitionOrder);
    }

    /// <summary>
    /// 更新状态（按任务 + 任务状态）
    /// </summary>
    /// <param name="task">任务类型</param>
    /// <param name="taskStatus">任务状态</param>
    public void ChangeStatus(string task, string taskStatus)
    {
        if (task == nameof(TaskEnum.Check))
        {
            if (taskStatus != nameof(UserTaskStatusEnum.Approve))
            {
                Status = nameof(PurchaseRequisitionOrderStatusEnum.ApprovalRejected);
            }

            return;
        }

        if (task == nameof(TaskEnum.Approve))
        {
            if (taskStatus == nameof(UserTaskStatusEnum.Approve))
            {
                Status = nameof(PurchaseRequisitionOrderStatusEnum.PendingPurchaseCheck);
            }
            else
            {
                Status = nameof(PurchaseRequisitionOrderStatusEnum.ApprovalRejected);
            }
        }
    }

    /// <summary>
    /// 按Id查找申购单项
    /// </summary>
    /// <param name="itemId">申购单项Id</param>
    /// <returns>申购单项（可空）</returns>
    private PurchaseRequisitionOrderItem? FindItem(long itemId)
    {
        return _purchaseRequisitionOrderItems.FirstOrDefault(x => x.Id == itemId);
    }

    /// <summary>
    /// 判断申购单项是否存在（按 skuId，未删除）
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <returns>true 表示存在</returns>
    private bool IsExistItem(long skuId)
    {
        return _purchaseRequisitionOrderItems.Any(x => x.SkuId == skuId && !x.IsDeleted);
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _purchaseRequisitionOrderItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总抽仓数量
    /// </summary>
    private void UpdateDeductQty()
    {
        DeductQty = _purchaseRequisitionOrderItems.Where(x => !x.IsDeleted).Sum(x => x.DeductQty);
    }

    /// <summary>
    /// 汇总订购数量
    /// </summary>
    private void UpdateCustomizedQty()
    {
        CustomizedQty = _purchaseRequisitionOrderItems.Where(x => !x.IsDeleted).Sum(x => x.CustomizedQty);
    }

    /// <summary>
    /// 汇总出库数量，并推进出库状态
    /// </summary>
    private void UpdateLeaveStockQty()
    {
        LeaveStockQty = _purchaseRequisitionOrderItems.Where(x => !x.IsDeleted).Sum(x => x.LeaveStockQty);
        if (LeaveStockQty == 0)
        {
            Status = nameof(PurchaseRequisitionOrderStatusEnum.Delivery);
            return;
        }

        double diff = Qty - LeaveStockQty;
        if (diff > 0)
        {
            Status = nameof(PurchaseRequisitionOrderStatusEnum.PartialDelivered);
            return;
        }

        if (diff == 0)
        {
            Status = nameof(PurchaseRequisitionOrderStatusEnum.AllDelivered);
            return;
        }

        Status = nameof(PurchaseRequisitionOrderStatusEnum.OverDelivered);
    }
}
