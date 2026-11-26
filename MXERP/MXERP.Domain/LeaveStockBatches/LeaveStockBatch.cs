using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次
/// </summary>
public class LeaveStockBatch : OrderAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;
    /// <summary>
    /// 出库计划Id（可空）
    /// </summary>
    public long? LeaveStockPlanId { get; private set; }
    /// <summary>
    /// 采购批次Id（可空）
    /// </summary>
    public long? PurchaseBatchId { get; private set; }
    /// <summary>
    /// 采购批次号（可空）
    /// </summary>
    public string? PurchaseBatchNumber { get; private set; }
    /// <summary>
    /// 关联类型
    /// </summary>
    public string RelationType { get; private set; } = string.Empty;
    /// <summary>
    /// 关联Id
    /// </summary>
    public long RelationId { get; private set; }
    /// <summary>
    /// 关联项Id（可空）
    /// </summary>
    public long? RelationItemId { get; private set; }
    /// <summary>
    /// 关联号
    /// </summary>
    public string RelationNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 业务单客户单号（可空）
    /// </summary>
    public string? RelationCustomerOrderNumber { get; private set; }
    /// <summary>
    /// 业务单客户PO号（可空）
    /// </summary>
    public string? RelationCustomerPONumber { get; private set; }
    /// <summary>
    /// 业务单公司订单编号（可空）
    /// </summary>
    public string? RelationOrderRef { get; private set; }
    /// <summary>
    /// 申购部门Id（可空）
    /// </summary>
    public long? DepartmentId { get; private set; }
    /// <summary>
    /// 门店Id（可空）
    /// </summary>
    public long? StoreId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }
    /// <summary>
    /// 开单片数
    /// </summary>
    public double BillingQty { get; private set; }
    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; private set; } = string.Empty;
    /// <summary>
    /// 是否打印
    /// </summary>
    public bool IsPrinted { get; private set; }
    /// <summary>
    /// 打印人Id（可空）
    /// </summary>
    public long? PrintedBy { get; private set; }
    /// <summary>
    /// 打印人（可空）
    /// </summary>
    public string? PrintedByName { get; private set; }
    /// <summary>
    /// 打印时间（可空）
    /// </summary>
    public DateTime? PrintedOn { get; private set; }
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
    /// 取消人（可空）
    /// </summary>
    public string? CanceledByName { get; private set; }
    /// <summary>
    /// 取消时间（可空）
    /// </summary>
    public DateTime? CanceledOn { get; private set; }
    /// <summary>
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 是否已解冻
    /// </summary>
    public bool IsThaw { get; private set; }

    /// <summary>
    /// 出库批次项列表
    /// </summary>
    private readonly List<LeaveStockBatchItem> _leaveStockBatchItems;

    /// <summary>
    /// 出库批次项列表
    /// </summary>
    public IReadOnlyCollection<LeaveStockBatchItem> LeaveStockBatchItems
    {
        get { return _leaveStockBatchItems.AsReadOnly(); }
    }

    /// <summary>
    /// 出库批次工序列表
    /// </summary>
    private readonly List<LeaveStockBatchWorkStep> _leaveStockBatchWorkSteps;

    /// <summary>
    /// 出库批次工序列表
    /// </summary>
    public IReadOnlyCollection<LeaveStockBatchWorkStep> LeaveStockBatchWorkSteps
    {
        get { return _leaveStockBatchWorkSteps.AsReadOnly(); }
    }

    /// <summary>
    /// 出库批次跟踪列表
    /// </summary>
    private readonly List<LeaveStockBatchTracking> _leaveStockBatchTrackings;

    /// <summary>
    /// 出库批次跟踪列表
    /// </summary>
    public IReadOnlyCollection<LeaveStockBatchTracking> LeaveStockBatchTrackings
    {
        get { return _leaveStockBatchTrackings.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected LeaveStockBatch()
    {
        _leaveStockBatchItems = new List<LeaveStockBatchItem>();
        _leaveStockBatchWorkSteps = new List<LeaveStockBatchWorkStep>();
        _leaveStockBatchTrackings = new List<LeaveStockBatchTracking>();
    }

    /// <summary>
    /// 构造器（门店场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="leaveStockPlanId">出库计划Id（可空）</param>
    /// <param name="purchaseBatchId">采购批次Id（可空）</param>
    /// <param name="purchaseBatchNumber">采购批次号（可空）</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="relationCustomerOrderNumber">业务单客户单号（可空）</param>
    /// <param name="relationCustomerPONumber">业务单客户PO号（可空）</param>
    /// <param name="relationOrderRef">业务单公司订单编号（可空）</param>
    /// <param name="remark">备注（可空）</param>
    internal LeaveStockBatch(string productType, long? leaveStockPlanId, long? purchaseBatchId, string? purchaseBatchNumber, long storeId, long companyId, string relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark)
        : this()
    {
        ProductType = productType;
        LeaveStockPlanId = leaveStockPlanId;
        PurchaseBatchId = purchaseBatchId;
        PurchaseBatchNumber = purchaseBatchNumber;
        StoreId = storeId;
        CompanyId = companyId;
        RelationType = relationType;
        RelationId = relationId;
        RelationItemId = relationItemId;
        RelationNumber = relationNumber;
        RelationCustomerOrderNumber = relationCustomerOrderNumber;
        RelationCustomerPONumber = relationCustomerPONumber;
        RelationOrderRef = relationOrderRef;
        Remark = remark;
        Status = nameof(LeaveStockBatchStatusEnum.Pending);
    }

    /// <summary>
    /// 构造器（部门场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="leaveStockPlanId">出库计划Id（可空）</param>
    /// <param name="purchaseBatchId">采购批次Id（可空）</param>
    /// <param name="purchaseBatchNumber">采购批次号（可空）</param>
    /// <param name="departmentId">申购部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="remark">备注（可空）</param>
    internal LeaveStockBatch(string productType, long? leaveStockPlanId, long? purchaseBatchId, string? purchaseBatchNumber, long departmentId, long companyId, string relationType, long relationId, string relationNumber, string? remark)
        : this()
    {
        ProductType = productType;
        LeaveStockPlanId = leaveStockPlanId;
        PurchaseBatchId = purchaseBatchId;
        PurchaseBatchNumber = purchaseBatchNumber;
        DepartmentId = departmentId;
        CompanyId = companyId;
        RelationType = relationType;
        RelationId = relationId;
        RelationNumber = relationNumber;
        Remark = remark;
        Status = nameof(LeaveStockBatchStatusEnum.Pending);
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注（可空）</param>
    public void Cancel(long userId, string name, string? remark)
    {
        if (nameof(LeaveStockBatchStatusEnum.Pending) != Status)
        {
            throw new UserFriendlyException("出库批次" + Number + "当前状态不允许取消!");
        }

        if (LeaveStockQty > 0)
        {
            throw new UserFriendlyException("出库批次" + Number + "已出库,不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = nameof(LeaveStockBatchStatusEnum.Canceled);
        AddDomainEvent(new LeaveStockBatchCanceledEvent(this));
        Audit(nameof(AuditActionEnum.Cancel), remark);
    }

    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="userId">打印人Id</param>
    /// <param name="name">打印人名称</param>
    public void Print(long userId, string name)
    {
        if (nameof(BusinessOrderTypeEnum.BulkOrder) != RelationType)
        {
            throw new UserFriendlyException("出库批次" + Number + "业务单类型不是批量订单,不允许打印!");
        }

        IsPrinted = true;
        PrintedBy = userId;
        PrintedByName = name;
        PrintedOn = DateTime.Now;
        Audit(nameof(AuditActionEnum.Print));
    }

    /// <summary>
    /// 添加项（无销售产品）
    /// </summary>
    /// <param name="purchaseBatchItemId">采购批次项Id（可空）</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(long? purchaseBatchItemId, long relationItemId, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        if (!IsExistItem(skuId, warehouseId))
        {
            LeaveStockBatchItem item = new LeaveStockBatchItem(purchaseBatchItemId, relationItemId, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes);
            _leaveStockBatchItems.Add(item);
            UpdateQty();
            AddDomainEvent(new LeaveStockBatchItemAddedEvent(this, LeaveStockPlanId, RelationType, RelationId, RelationItemId, purchaseBatchItemId, relationItemId, RelationNumber, qty));
        }
    }

    /// <summary>
    /// 添加项（带销售产品）
    /// </summary>
    /// <param name="purchaseBatchItemId">采购批次项Id（可空）</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id（可空）</param>
    /// <param name="customerSpuCode">客人产品Code（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(long? purchaseBatchItemId, long relationItemId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        if (nameof(BusinessOrderTypeEnum.BulkOrder) == RelationType && _leaveStockBatchItems.Count > 0)
        {
            bool hasSameSalesSpu = _leaveStockBatchItems.Any(x => x.SalesSpuId == salesSpuId);
            if (!hasSameSalesSpu)
            {
                throw new UserFriendlyException("关联类型为批量订单的出库批次不允许添加多个销售产品!");
            }

            bool hasSameSpuAndWarehouse = _leaveStockBatchItems.Any(x => x.SpuId == spuId && x.WarehouseId == warehouseId);
            if (!hasSameSpuAndWarehouse)
            {
                throw new UserFriendlyException("关联类型为批量订单的出库批次不允许添加多个不同产品或不同仓库!");
            }
        }

        if (!IsExistItem(salesSpuId, skuId, warehouseId))
        {
            LeaveStockBatchItem item = new LeaveStockBatchItem(purchaseBatchItemId, relationItemId, salesSpuId, salesSpuCode, customerSpuId, customerSpuCode, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes);
            _leaveStockBatchItems.Add(item);
            UpdateQty();
            AddDomainEvent(new LeaveStockBatchItemAddedEvent(this, LeaveStockPlanId, RelationType, RelationId, RelationItemId, purchaseBatchItemId, relationItemId, RelationNumber, qty));
        }
    }

    /// <summary>
    /// 添加工序
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    public void AddWorkStep(long workStepId)
    {
        if (!IsExistWorkStep(workStepId))
        {
            _leaveStockBatchWorkSteps.Add(new LeaveStockBatchWorkStep(workStepId));
        }
    }

    /// <summary>
    /// 删除工序（同时软删该工序下的所有跟踪记录）
    /// </summary>
    /// <param name="leaveStockBatchWorkStepId">出库批次工序Id</param>
    public void DeleteWorkStep(long leaveStockBatchWorkStepId)
    {
        LeaveStockBatchWorkStep? item = _leaveStockBatchWorkSteps.FirstOrDefault(x => !x.IsDeleted && x.Id == leaveStockBatchWorkStepId);
        if (item is null)
        {
            return;
        }

        item.Delete();

        List<LeaveStockBatchTracking> trackings = _leaveStockBatchTrackings.Where(x => !x.IsDeleted && x.WorkStepId == item.WorkStepId).ToList();
        foreach (LeaveStockBatchTracking tracking in trackings)
        {
            tracking.Delete();
        }
    }

    /// <summary>
    /// 添加跟踪（按工序）
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal void AddTracking(long workStepId, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        if (nameof(LeaveStockBatchStatusEnum.Canceled) == Status)
        {
            throw new UserFriendlyException("出库批次" + Number + "已取消,不允许录入!");
        }

        _leaveStockBatchTrackings.Add(new LeaveStockBatchTracking(workStepId, employeeId, outDepartmentId, inDepartmentId));
    }

    /// <summary>
    /// 添加跟踪（按原因）
    /// </summary>
    /// <param name="reason">原因（可空）</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal void AddTracking(string? reason, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        _leaveStockBatchTrackings.Add(new LeaveStockBatchTracking(reason, employeeId, outDepartmentId, inDepartmentId));
    }

    /// <summary>
    /// 更新出库数量
    /// </summary>
    /// <param name="itemId">出库批次项Id</param>
    /// <param name="qty">数量</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    internal void UpdateLeaveStockQty(long itemId, double qty, long userId, string userName)
    {
        if (qty == 0)
        {
            return;
        }

        if (BillingQty > 0)
        {
            throw new UserFriendlyException("出库批次已开单,不允许添加出库数量!");
        }

        LeaveStockBatchItem? item = _leaveStockBatchItems.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);
        if (item is not null)
        {
            item.AddLeaveStockQty(qty);
            UpdateLeaveStockQtyTotal();

            if (!IsThaw && (nameof(BusinessOrderTypeEnum.BulkOrder) == RelationType || nameof(BusinessOrderTypeEnum.PurchaseRequisitionOrder) == RelationType))
            {
                IsThaw = true;
                AddDomainEvent(new LeaveStockBatchThawStockEvent(this));
            }
        }

        if (LeaveStockQty == 0)
        {
            UnFinish();
        }
        else if (Qty - LeaveStockQty >= 0)
        {
            Finish(userId, userName);
        }
    }

    /// <summary>
    /// 更新开单数量（正数=开单，负数=减去开单）
    /// </summary>
    /// <param name="salesSpuId">销售产品Id（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="qty">数量</param>
    public void UpdateBillingQty(long? salesSpuId, long spuId, double qty)
    {
        if (qty == 0)
        {
            return;
        }

        List<LeaveStockBatchItem> items = _leaveStockBatchItems.Where(x => x.SalesSpuId == salesSpuId && x.SpuId == spuId).ToList();

        bool isAdd = qty > 0;
        double remaining = Math.Abs(qty);

        foreach (LeaveStockBatchItem item in items)
        {
            double billingQty;
            if (isAdd)
            {
                billingQty = Math.Min(remaining, item.LeaveStockQty - item.BillingQty);
                if (billingQty > 0)
                {
                    item.AddBillingQty(billingQty);
                }
            }
            else
            {
                billingQty = Math.Min(remaining, item.BillingQty);
                if (billingQty > 0)
                {
                    item.SubtractBillingQty(billingQty);
                }
            }

            remaining -= billingQty;
            if (remaining == 0)
            {
                break;
            }
        }

        if (remaining > 0)
        {
            if (isAdd)
            {
                throw new UserFriendlyException("开单数量不允许超出出库批次" + Number + "出库数量!");
            }

            throw new UserFriendlyException("减去开单数量不允许超出出库批次" + Number + "开单数量!");
        }

        UpdateBillingQtyTotal();
    }

    /// <summary>
    /// 完成
    /// </summary>
    /// <param name="userId">完成人Id</param>
    /// <param name="name">完成人名称</param>
    internal void Finish(long userId, string name)
    {
        Status = nameof(LeaveStockBatchStatusEnum.Finished);
        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        if (!HasDomainEvent<LeaveStockBatchFinishedEvent>())
        {
            AddDomainEvent(new LeaveStockBatchFinishedEvent(this));
        }
    }

    /// <summary>
    /// 弃完
    /// </summary>
    internal void UnFinish()
    {
        Status = nameof(LeaveStockBatchStatusEnum.Pending);
        FinishedBy = null;
        FinishedByName = null;
        FinishedOn = null;
        if (!HasDomainEvent<LeaveStockBatchUnFinishedEvent>())
        {
            AddDomainEvent(new LeaveStockBatchUnFinishedEvent(this));
        }
    }

    /// <summary>
    /// 获取流水号类型
    /// </summary>
    /// <returns>流水号类型</returns>
    public override string GetSerialNumberType()
    {
        if (nameof(ProductTypeEnum.SemiProduct) == ProductType)
        {
            return nameof(SerialNumberTypeEnum.SemiProductLeaveStockBatch);
        }

        if (nameof(ProductTypeEnum.EndProduct) == ProductType)
        {
            return nameof(SerialNumberTypeEnum.EndProductLeaveStockBatch);
        }

        return nameof(SerialNumberTypeEnum.AccessoryLeaveStockBatch);
    }

    /// <summary>
    /// 当前聚合根是否已存在指定类型的待发布领域事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <returns>已存在返回 true</returns>
    private bool HasDomainEvent<TEvent>()
    {
        return DomainEvents.Any(x => x is TEvent);
    }

    /// <summary>
    /// 判断出库批次项是否存在（按 skuId + warehouseId，未删除）
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>true：存在</returns>
    private bool IsExistItem(long skuId, long warehouseId)
    {
        return _leaveStockBatchItems.Any(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 判断出库批次项是否存在（按 salesSpuId + skuId + warehouseId，未删除）
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>true：存在</returns>
    private bool IsExistItem(long salesSpuId, long skuId, long warehouseId)
    {
        return _leaveStockBatchItems.Any(x => x.SalesSpuId == salesSpuId && x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 判断工序是否存在（未删除）
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <returns>true：存在</returns>
    private bool IsExistWorkStep(long workStepId)
    {
        return _leaveStockBatchWorkSteps.Any(x => x.WorkStepId == workStepId && !x.IsDeleted);
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _leaveStockBatchItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总出库数量，并校验不允许超出批次数量
    /// </summary>
    private void UpdateLeaveStockQtyTotal()
    {
        LeaveStockQty = _leaveStockBatchItems.Where(x => !x.IsDeleted).Sum(x => x.LeaveStockQty);
        if (Qty - LeaveStockQty < 0)
        {
            throw new UserFriendlyException("出库数量不允许超出出库批次数量!");
        }
    }

    /// <summary>
    /// 汇总开单数量，并推进状态
    /// </summary>
    private void UpdateBillingQtyTotal()
    {
        BillingQty = _leaveStockBatchItems.Where(x => !x.IsDeleted).Sum(x => x.BillingQty);
        if (BillingQty == LeaveStockQty)
        {
            Status = nameof(LeaveStockBatchStatusEnum.BillIssued);
        }
        else if (BillingQty < LeaveStockQty)
        {
            Status = nameof(LeaveStockBatchStatusEnum.Finished);
        }
        else
        {
            throw new UserFriendlyException("开单数量不允许大于出库数量!");
        }
    }
}
