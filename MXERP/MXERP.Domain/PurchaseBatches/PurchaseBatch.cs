using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次
/// </summary>
public class PurchaseBatch : OrderAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;
    /// <summary>
    /// 采购单号（可空）
    /// </summary>
    public string? PurchaseOrderNumber { get; private set; }
    /// <summary>
    /// 采购计划Id（可空）
    /// </summary>
    public long? PurchasePlanId { get; private set; }
    /// <summary>
    /// 关联类型（可空）
    /// </summary>
    public string? RelationType { get; private set; }
    /// <summary>
    /// 关联Id（可空）
    /// </summary>
    public long? RelationId { get; private set; }
    /// <summary>
    /// 关联项Id（可空）
    /// </summary>
    public long? RelationItemId { get; private set; }
    /// <summary>
    /// 关联号（可空）
    /// </summary>
    public string? RelationNumber { get; private set; }
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
    /// 部门Id（可空）
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
    /// 采购数量
    /// </summary>
    public double PurchaseQty { get; private set; }
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
    /// 采购批次项列表
    /// </summary>
    private readonly List<PurchaseBatchItem> _purchaseBatchItems;

    /// <summary>
    /// 采购批次项列表
    /// </summary>
    public IReadOnlyCollection<PurchaseBatchItem> PurchaseBatchItems
    {
        get { return _purchaseBatchItems.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected PurchaseBatch()
    {
        _purchaseBatchItems = new List<PurchaseBatchItem>();
    }

    /// <summary>
    /// 构造器（门店场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="purchasePlanId">采购计划Id（可空）</param>
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
    internal PurchaseBatch(string productType, long? purchasePlanId, long storeId, long companyId, string relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark)
        : this()
    {
        ProductType = productType;
        PurchasePlanId = purchasePlanId;
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
        Status = nameof(PurchaseBatchStatusEnum.Pending);
    }

    /// <summary>
    /// 构造器（部门场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="purchasePlanId">采购计划Id（可空）</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="remark">备注（可空）</param>
    internal PurchaseBatch(string productType, long? purchasePlanId, long departmentId, long companyId, string relationType, long relationId, string relationNumber, string? remark)
        : this(productType, departmentId, companyId, remark)
    {
        PurchasePlanId = purchasePlanId;
        RelationType = relationType;
        RelationId = relationId;
        RelationNumber = relationNumber;
    }

    /// <summary>
    /// 构造器（部门场景，无关联）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="remark">备注（可空）</param>
    internal PurchaseBatch(string productType, long departmentId, long companyId, string? remark)
        : this()
    {
        ProductType = productType;
        DepartmentId = departmentId;
        CompanyId = companyId;
        Remark = remark;
        Status = nameof(PurchaseBatchStatusEnum.Pending);
    }

    /// <summary>
    /// 审核
    /// </summary>
    /// <param name="userId">审核人Id</param>
    /// <param name="name">审核人名称</param>
    /// <param name="remark">备注（可空）</param>
    public void Check(long userId, string name, string? remark)
    {
        if (Status != nameof(PurchaseBatchStatusEnum.Pending))
        {
            throw new UserFriendlyException("采购批次" + Number + "当前状态不允许审核!");
        }

        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        PurchaseOrderNumber = remark;
        Status = nameof(PurchaseBatchStatusEnum.Finished);
        Audit(nameof(AuditActionEnum.Finish), remark);
    }

    /// <summary>
    /// 弃审
    /// </summary>
    /// <param name="remark">备注（可空）</param>
    internal void UnCheck(string? remark)
    {
        if (Status != nameof(PurchaseBatchStatusEnum.Finished))
        {
            throw new UserFriendlyException("采购批次" + Number + "当前状态不允许取消!");
        }

        FinishedBy = null;
        FinishedByName = null;
        FinishedOn = null;
        Status = nameof(PurchaseBatchStatusEnum.Pending);
        Audit(nameof(AuditActionEnum.UnCheck), remark);
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注（可空）</param>
    public void Cancel(long userId, string name, string? remark)
    {
        if (Status != nameof(PurchaseBatchStatusEnum.Pending))
        {
            throw new UserFriendlyException("采购批次" + Number + "当前状态不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = nameof(PurchaseBatchStatusEnum.Canceled);
        AddDomainEvent(new PurchaseBatchCanceledEvent(this));
        Audit(nameof(AuditActionEnum.Cancel), remark);
    }

    /// <summary>
    /// 添加项（无销售产品）
    /// </summary>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(long? relationItemId, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        if (!IsExistItem(skuId, warehouseId))
        {
            PurchaseBatchItem item = new PurchaseBatchItem(relationItemId, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes);
            _purchaseBatchItems.Add(item);
        }

        UpdateQty();
    }

    /// <summary>
    /// 添加项（带销售产品）
    /// </summary>
    /// <param name="relationItemId">关联项Id（可空）</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="customerSpuId">客人产品Id（可空）</param>
    /// <param name="customerSpuCode">客人产品Code（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(long? relationItemId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        if (_purchaseBatchItems.Count > 0 && RelationType == nameof(BusinessOrderTypeEnum.BulkOrder))
        {
            bool hasSameSalesSpu = _purchaseBatchItems.Any(x => x.SalesSpuId is not null && x.SalesSpuId == salesSpuId);
            if (!hasSameSalesSpu)
            {
                throw new UserFriendlyException("关联类型为批量订单的采购批次不允许添加多个销售产品!");
            }

            bool hasSameSpuAndWarehouse = _purchaseBatchItems.Any(x => x.SpuId == spuId && x.WarehouseId == warehouseId);
            if (!hasSameSpuAndWarehouse)
            {
                throw new UserFriendlyException("关联类型为批量订单的采购批次不允许添加多个不同产品或不同仓库!");
            }
        }

        if (!IsExistItem(salesSpuId, skuId, warehouseId))
        {
            PurchaseBatchItem item = new PurchaseBatchItem(relationItemId, salesSpuId, salesSpuCode, customerSpuId, customerSpuCode, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes);
            _purchaseBatchItems.Add(item);
            UpdateQty();
        }
    }

    /// <summary>
    /// 更新采购数量（正数=添加，负数=减去）
    /// </summary>
    /// <param name="itemId">采购批次项Id</param>
    /// <param name="qty">数量</param>
    public void UpdatePurchaseQty(long itemId, double qty)
    {
        if (qty == 0)
        {
            return;
        }

        PurchaseBatchItem? item = _purchaseBatchItems.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);
        if (item is not null)
        {
            item.UpdatePurchaseQty(qty);
            UpdatePurchaseQty();
        }
    }

    /// <summary>
    /// 更新出库数量（正数=添加，负数=减去）
    /// </summary>
    /// <param name="itemId">采购批次项Id</param>
    /// <param name="qty">数量</param>
    public void UpdateLeaveStockQty(long itemId, double qty)
    {
        if (qty == 0)
        {
            return;
        }

        PurchaseBatchItem? item = _purchaseBatchItems.FirstOrDefault(x => x.Id == itemId && !x.IsDeleted);
        if (item is not null)
        {
            item.UpdateLeaveStockQty(qty);
            UpdateLeaveStockQty();
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
    /// 获取流水号类型
    /// </summary>
    /// <returns>流水号类型</returns>
    public override string GetSerialNumberType()
    {
        if (ProductType == nameof(ProductTypeEnum.SemiProduct))
        {
            return nameof(SerialNumberTypeEnum.SemiProductPurchaseBatch);
        }

        if (ProductType == nameof(ProductTypeEnum.EndProduct))
        {
            return nameof(SerialNumberTypeEnum.EndProductPurchaseBatch);
        }

        return nameof(SerialNumberTypeEnum.AccessoryPurchaseBatch);
    }

    /// <summary>
    /// 判断采购批次项是否存在（按SkuId+仓库Id, 未删除）
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistItem(long skuId, long warehouseId)
    {
        return _purchaseBatchItems.Any(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 判断采购批次项是否存在（按销售产品Id+SkuId+仓库Id, 未删除）
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistItem(long salesSpuId, long skuId, long warehouseId)
    {
        return _purchaseBatchItems.Any(x => x.SalesSpuId is not null && x.SalesSpuId == salesSpuId && x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _purchaseBatchItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总采购数量, 并推进状态
    /// </summary>
    private void UpdatePurchaseQty()
    {
        PurchaseQty = _purchaseBatchItems.Where(x => !x.IsDeleted).Sum(x => x.PurchaseQty);
        if (PurchaseQty == 0)
        {
            Status = nameof(PurchaseBatchStatusEnum.Pending);
        }
        else if (Qty - PurchaseQty > 0)
        {
            Status = nameof(PurchaseBatchStatusEnum.PartialPurchased);
        }
        else if (Qty - PurchaseQty == 0)
        {
            Status = nameof(PurchaseBatchStatusEnum.AllPurchased);
        }
        else
        {
            Status = nameof(PurchaseBatchStatusEnum.OverPurchased);
        }
    }

    /// <summary>
    /// 汇总出库数量
    /// </summary>
    private void UpdateLeaveStockQty()
    {
        LeaveStockQty = _purchaseBatchItems.Where(x => !x.IsDeleted).Sum(x => x.LeaveStockQty);
    }
}
