using MXERP.Domain.Common.Enums;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockPlans;

/// <summary>
/// 出库计划
/// </summary>
public class LeaveStockPlan : AuditedAggregateRoot
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
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 出库计划项列表
    /// </summary>
    private readonly List<LeaveStockPlanItem> _leaveStockPlanItems;

    /// <summary>
    /// 出库计划项列表
    /// </summary>
    public IReadOnlyCollection<LeaveStockPlanItem> LeaveStockPlanItems
    {
        get { return _leaveStockPlanItems.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected LeaveStockPlan()
    {
        _leaveStockPlanItems = new List<LeaveStockPlanItem>();
    }

    /// <summary>
    /// 构造器（门店场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
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
    internal LeaveStockPlan(ProductTypeEnum productType, long storeId, long companyId, BusinessOrderTypeEnum relationType, long relationId, long? relationItemId, string relationNumber, string? relationCustomerOrderNumber, string? relationCustomerPONumber, string? relationOrderRef, string? remark)
        : this()
    {
        ProductType = productType;
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
    }

    /// <summary>
    /// 构造器（部门场景）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="remark">备注（可空）</param>
    internal LeaveStockPlan(ProductTypeEnum productType, long departmentId, long companyId, BusinessOrderTypeEnum relationType, long relationId, string relationNumber, string? remark)
        : this()
    {
        ProductType = productType;
        DepartmentId = departmentId;
        CompanyId = companyId;
        RelationType = relationType;
        RelationId = relationId;
        RelationNumber = relationNumber;
        Remark = remark;
    }

    /// <summary>
    /// 添加项（无销售产品）
    /// </summary>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="skuAttributes">Sku属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(long relationItemId, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        if (IsExistItem(skuId, warehouseId))
        {
            throw new UserFriendlyException("出库计划项已存在!");
        }

        LeaveStockPlanItem item = new LeaveStockPlanItem(relationItemId, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes);
        _leaveStockPlanItems.Add(item);
        UpdateQty();
        AddDomainEvent(new LeaveStockPlanItemAddedEvent(this, RelationType, RelationId, RelationItemId, item.RelationItemId, skuId, warehouseId, qty));
    }

    /// <summary>
    /// 添加项（带销售产品）
    /// </summary>
    /// <param name="relationItemId">关联项Id</param>
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
    public void AddItem(long relationItemId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long skuId, long warehouseId, double qty, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty <= 0)
        {
            return;
        }

        if (IsExistItem(skuId, warehouseId))
        {
            throw new UserFriendlyException("出库计划项已存在!");
        }

        LeaveStockPlanItem item = new LeaveStockPlanItem(relationItemId, salesSpuId, salesSpuCode, customerSpuId, customerSpuCode, spuId, skuId, warehouseId, qty, skuAttributes, otherAttributes);
        _leaveStockPlanItems.Add(item);
        UpdateQty();
        AddDomainEvent(new LeaveStockPlanItemAddedEvent(this, RelationType, RelationId, RelationItemId, item.RelationItemId, skuId, warehouseId, qty));

        if (BusinessOrderTypeEnum.BulkOrder == RelationType)
        {
            int distinctSalesSpuCount = _leaveStockPlanItems.Select(x => x.SalesSpuId).Distinct().Count();
            if (distinctSalesSpuCount > 1)
            {
                throw new UserFriendlyException("关联类型为批量订单的出库计划不允许添加多个销售产品!");
            }
        }
    }

    /// <summary>
    /// 删除项（标记删除）
    /// </summary>
    /// <param name="itemId">出库计划项Id</param>
    public void DeleteItem(long itemId)
    {
        LeaveStockPlanItem? item = _leaveStockPlanItems.FirstOrDefault(x => x.Id == itemId);
        if (item is null)
        {
            return;
        }

        item.Delete();
        UpdateQty();
        AddDomainEvent(new LeaveStockPlanItemDeletedEvent(this, RelationType, RelationId, RelationItemId, item.RelationItemId, item.SkuId, item.WarehouseId, item.Qty));
    }

    /// <summary>
    /// 判断出库计划项是否存在（按 skuId + warehouseId，未删除）
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>true 表示存在</returns>
    private bool IsExistItem(long skuId, long warehouseId)
    {
        return _leaveStockPlanItems.Any(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _leaveStockPlanItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }
}
