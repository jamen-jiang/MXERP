using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单
/// </summary>
public class LeaveStockOrder : OrderAggregateRoot
{
    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveStockOrder()
    {
        _leaveStockOrderItems = new List<LeaveStockOrderItem>();
    }

    /// <summary>
    /// 业务构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="originalId">原出库单Id（可空）</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    internal LeaveStockOrder(ProductTypeEnum productType, long? originalId, LeaveStockBusinessTypeEnum businessType, string? remark)
        : this()
    {
        ProductType = productType;
        OriginalId = originalId;
        BusinessType = businessType;
        Remark = remark;
        Status = LeaveStockOrderStatusEnum.Pending;
    }

    /// <summary>
    /// 业务构造器（带关联类型）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="originalId">原出库单Id（可空）</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    internal LeaveStockOrder(ProductTypeEnum productType, long? originalId, BusinessOrderTypeEnum relationType, LeaveStockBusinessTypeEnum businessType, string? remark)
        : this(productType, originalId, businessType, remark)
    {
        RelationType = relationType;
    }

    /// <summary>
    /// 产品类型
    /// </summary>
    public ProductTypeEnum ProductType { get; private set; }

    /// <summary>
    /// 原出库单Id（可空）
    /// </summary>
    public long? OriginalId { get; private set; }

    /// <summary>
    /// 关联类型（可空）
    /// </summary>
    public BusinessOrderTypeEnum? RelationType { get; private set; }

    /// <summary>
    /// 业务类型
    /// </summary>
    public LeaveStockBusinessTypeEnum BusinessType { get; private set; }

    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }

    /// <summary>
    /// 冲销数量
    /// </summary>
    public double ReversalQty { get; private set; }

    /// <summary>
    /// 出库状态
    /// </summary>
    public LeaveStockOrderStatusEnum Status { get; private set; }

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
    /// 出库单项列表
    /// </summary>
    private readonly List<LeaveStockOrderItem> _leaveStockOrderItems;

    /// <summary>
    /// 出库单项列表
    /// </summary>
    public IReadOnlyCollection<LeaveStockOrderItem> LeaveStockOrderItems
    {
        get { return _leaveStockOrderItems.AsReadOnly(); }
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    /// <exception cref="UserFriendlyException">当前状态不允许修改时抛出</exception>
    public void Modify(LeaveStockBusinessTypeEnum businessType, string? remark)
    {
        if (LeaveStockOrderStatusEnum.Pending != Status)
        {
            throw new UserFriendlyException($"出库单{Number}当前状态不允许修改!");
        }
        BusinessType = businessType;
        Remark = remark;
        Audit(AuditActionEnum.Modify);
    }

    /// <summary>
    /// 完成
    /// </summary>
    /// <param name="userId">完成人Id</param>
    /// <param name="name">完成人名称</param>
    /// <exception cref="UserFriendlyException">出库数量等于0 / 当前状态不允许审核时抛出</exception>
    public void Finish(long userId, string name)
    {
        if (Qty == 0)
        {
            throw new UserFriendlyException($"出库单{Number}出库数量不允许等于0!");
        }
        if (LeaveStockOrderStatusEnum.Pending != Status)
        {
            throw new UserFriendlyException($"出库单{Number}当前状态不允许审核!");
        }
        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        Status = LeaveStockOrderStatusEnum.Finished;
        AddDomainEvent(new LeaveStockOrderFinishedEvent(this));
        Audit(AuditActionEnum.Finish);
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注（可空）</param>
    /// <exception cref="UserFriendlyException">关联单生成不允许取消 / 当前状态不允许取消时抛出</exception>
    public void Cancel(long userId, string name, string? remark)
    {
        if (RelationType is not null)
        {
            throw new UserFriendlyException("不允许取消关联单生成的出库单!");
        }
        if (LeaveStockOrderStatusEnum.Pending != Status)
        {
            throw new UserFriendlyException($"出库单{Number}当前状态不允许取消!");
        }
        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = LeaveStockOrderStatusEnum.Canceled;
        AddDomainEvent(new LeaveStockOrderCanceledEvent(this));
        Audit(AuditActionEnum.Cancel, remark);
    }

    /// <summary>
    /// 添加项（带关联）
    /// </summary>
    /// <param name="originalId">原出库单项Id（可空）</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    /// <exception cref="UserFriendlyException">红冲/正常出库单数量方向错误时抛出</exception>
    internal void AddItem(long? originalId, BusinessOrderTypeEnum relationType, long relationId, long relationItemId, string relationNumber, long spuId, long skuId, long warehouseId, double qty, decimal price, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty == 0)
        {
            return;
        }
        ValidateQtyDirection(qty);
        LeaveStockOrderItem? item = _leaveStockOrderItems.FirstOrDefault(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
        if (item != null)
        {
            item.AddQty(qty);
        }
        else
        {
            item = new LeaveStockOrderItem(originalId, spuId, skuId, warehouseId, qty, price, skuAttributes, otherAttributes);
            _leaveStockOrderItems.Add(item);
        }
        item.AddLeaveStockOrderRelation(relationType, relationId, relationItemId, relationNumber, qty);
        UpdateQty();
    }

    /// <summary>
    /// 添加项（带关联，无原出库单项Id）
    /// </summary>
    /// <param name="relationType">关联类型</param>
    /// <param name="relationId">关联Id</param>
    /// <param name="relationItemId">关联项Id</param>
    /// <param name="relationNumber">关联号</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(BusinessOrderTypeEnum relationType, long relationId, long relationItemId, string relationNumber, long spuId, long skuId, long warehouseId, double qty, decimal price, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        AddItem(null, relationType, relationId, relationItemId, relationNumber, spuId, skuId, warehouseId, qty, price, skuAttributes, otherAttributes);
    }

    /// <summary>
    /// 添加项（无关联，带原出库单项Id）
    /// </summary>
    /// <param name="originalId">原出库单项Id（可空）</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    /// <exception cref="UserFriendlyException">红冲/正常出库单数量方向错误 / 系统生成单不允许添加 / 项已存在时抛出</exception>
    internal void AddItem(long? originalId, long spuId, long skuId, long warehouseId, double qty, decimal price, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        if (qty == 0)
        {
            return;
        }
        ValidateQtyDirection(qty);
        if (RelationType is not null)
        {
            throw new UserFriendlyException("系统生成的出库单不允许添加出库单项!");
        }
        if (IsExistItem(skuId, warehouseId))
        {
            throw new UserFriendlyException($"产品Sku {skuId}仓库 {warehouseId}已存在!");
        }
        LeaveStockOrderItem item = new LeaveStockOrderItem(originalId, spuId, skuId, warehouseId, qty, price, skuAttributes, otherAttributes);
        _leaveStockOrderItems.Add(item);
        UpdateQty();
    }

    /// <summary>
    /// 添加项（无关联，无原出库单项Id）
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <param name="price">价格</param>
    /// <param name="skuAttributes">Sku 属性值（可空）</param>
    /// <param name="otherAttributes">非关键属性值（可空）</param>
    public void AddItem(long spuId, long skuId, long warehouseId, double qty, decimal price, SortedDictionary<string, object?>? skuAttributes, SortedDictionary<string, object?>? otherAttributes)
    {
        AddItem(null, spuId, skuId, warehouseId, qty, price, skuAttributes, otherAttributes);
    }

    /// <summary>
    /// 修改项
    /// </summary>
    /// <param name="itemId">出库单项Id</param>
    /// <param name="qty">数量</param>
    /// <exception cref="UserFriendlyException">系统生成单不允许修改 / 项不存在时抛出</exception>
    public void ModifyItem(long itemId, double qty)
    {
        if (RelationType is not null)
        {
            throw new UserFriendlyException("系统生成的出库单不允许修改出库单项!");
        }
        LeaveStockOrderItem? item = _leaveStockOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
        {
            throw new UserFriendlyException($"出库单{Number}项{itemId}不存在!");
        }
        item.Modify(qty);
        UpdateQty();
    }

    /// <summary>
    /// 删除项
    /// </summary>
    /// <param name="itemId">出库单项Id</param>
    /// <exception cref="UserFriendlyException">系统生成单不允许删除 / 项不存在时抛出</exception>
    public void DeleteItem(long itemId)
    {
        if (RelationType is not null)
        {
            throw new UserFriendlyException("系统生成的出库单不允许删除出库项!");
        }
        LeaveStockOrderItem? item = _leaveStockOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
        {
            throw new UserFriendlyException($"出库单{Number}项{itemId}不存在!");
        }
        item.Delete();
        UpdateQty();
    }

    /// <summary>
    /// 更新冲销数量（正数=添加，负数=减去）
    /// </summary>
    /// <param name="itemId">出库单项Id</param>
    /// <param name="qty">数量</param>
    /// <exception cref="UserFriendlyException">当前状态不允许冲销 / 项不存在时抛出</exception>
    public void UpdateReversalQty(long itemId, double qty)
    {
        if (LeaveStockOrderStatusEnum.Finished != Status)
        {
            throw new UserFriendlyException($"出库单{Number}当前状态不允许冲销!");
        }
        LeaveStockOrderItem? item = _leaveStockOrderItems.FirstOrDefault(x => x.Id == itemId);
        if (item == null)
        {
            throw new UserFriendlyException($"出库单{Number}项{itemId}不存在!");
        }
        item.UpdateReversalQty(qty);
        UpdateReversalQty();
    }

    /// <summary>
    /// 序列号类型
    /// </summary>
    /// <returns>序列号类型</returns>
    public override SerialNumberTypeEnum GetSerialNumberType()
    {
        if (ProductTypeEnum.SemiProduct == ProductType)
        {
            return SerialNumberTypeEnum.SemiProductLeaveStockOrder;
        }
        if (ProductTypeEnum.EndProduct == ProductType)
        {
            return SerialNumberTypeEnum.EndProductLeaveStockOrder;
        }
        return SerialNumberTypeEnum.AccessoryLeaveStockOrder;
    }

    /// <summary>
    /// 校验数量方向
    /// </summary>
    /// <param name="qty">数量</param>
    /// <exception cref="UserFriendlyException">红冲/正常出库单数量方向错误时抛出</exception>
    private void ValidateQtyDirection(double qty)
    {
        if (OriginalId != null && qty > 0)
        {
            throw new UserFriendlyException("红冲出库单数量必须是负数!");
        }
        if (OriginalId == null && qty < 0)
        {
            throw new UserFriendlyException("正常出库单数量必须是正数!");
        }
    }

    /// <summary>
    /// 判断出库单项是否存在（按 skuId + warehouseId，未删除）
    /// </summary>
    /// <param name="skuId">产品SkuId</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>true：存在</returns>
    private bool IsExistItem(long skuId, long warehouseId)
    {
        return _leaveStockOrderItems.Any(x => x.SkuId == skuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 汇总数量
    /// </summary>
    private void UpdateQty()
    {
        Qty = _leaveStockOrderItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    /// <summary>
    /// 汇总冲销数量，并校验不允许超出出库数量
    /// </summary>
    /// <exception cref="UserFriendlyException">冲销数量超出出库数量时抛出</exception>
    private void UpdateReversalQty()
    {
        ReversalQty = _leaveStockOrderItems.Where(x => !x.IsDeleted).Sum(x => x.ReversalQty);
        if (ReversalQty > Qty)
        {
            throw new UserFriendlyException("冲销数量不能超出出库数量!");
        }
    }
}