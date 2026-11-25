using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品
/// </summary>
public class SalesSpu : AuditedAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;

    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }

    /// <summary>
    /// 销售产品代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 销售产品名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 销售产品描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// 批量订单包装信息
    /// </summary>
    public string? BulkOrderPacketInfo { get; private set; }

    /// <summary>
    /// 散镜订单包装信息
    /// </summary>
    public string? RetailOrderPacketInfo { get; private set; }

    /// <summary>
    /// 属性值
    /// </summary>
    public SortedDictionary<string, object?>? Attributes { get; private set; }

    /// <summary>
    /// 分配项列表
    /// </summary>
    public virtual ICollection<SalesSpuAssignItem> SalesSpuAssignItems { get; private set; } = new List<SalesSpuAssignItem>();

    /// <summary>
    /// 工序列表
    /// </summary>
    public virtual ICollection<SalesSpuWorkStep> SalesSpuWorkSteps { get; private set; } = new List<SalesSpuWorkStep>();

    /// <summary>
    /// 路线列表
    /// </summary>
    public virtual ICollection<SalesSpuRouting> SalesSpuRoutings { get; private set; } = new List<SalesSpuRouting>();

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected SalesSpu()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">销售产品代码</param>
    /// <param name="name">销售产品名称</param>
    /// <param name="description">销售产品描述</param>
    /// <param name="bulkOrderPacketInfo">批量订单包装信息</param>
    /// <param name="retailOrderPacketInfo">散镜订单包装信息</param>
    /// <param name="attributes">属性值</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="markingAttachmentIds">打标附件Id集合</param>
    internal SalesSpu(
        string productType,
        long companyId,
        string code,
        string name,
        string? description,
        string? bulkOrderPacketInfo,
        string? retailOrderPacketInfo,
        SortedDictionary<string, object?>? attributes,
        IReadOnlyCollection<long>? attachmentIds,
        IReadOnlyCollection<long>? markingAttachmentIds)
    {
        ValidateProductType(productType);
        ProductType = productType;
        CompanyId = companyId;
        Code = code.Trim();
        Name = name.Trim();
        Description = description;
        BulkOrderPacketInfo = !string.IsNullOrWhiteSpace(bulkOrderPacketInfo) ? bulkOrderPacketInfo : retailOrderPacketInfo;
        RetailOrderPacketInfo = !string.IsNullOrWhiteSpace(retailOrderPacketInfo) ? retailOrderPacketInfo : bulkOrderPacketInfo;
        ChangeAttributes(attributes);
        AddDomainEvent(new SalesSpuSavedEvent(this, attachmentIds, markingAttachmentIds));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="code">销售产品代码</param>
    /// <param name="name">销售产品名称</param>
    /// <param name="description">销售产品描述</param>
    /// <param name="bulkOrderPacketInfo">批量订单包装信息</param>
    /// <param name="retailOrderPacketInfo">散镜订单包装信息</param>
    /// <param name="attributes">属性值</param>
    /// <param name="attachmentIds">附件Id集合</param>
    /// <param name="markingAttachmentIds">打标附件Id集合</param>
    internal void Modify(
        string code,
        string name,
        string? description,
        string? bulkOrderPacketInfo,
        string? retailOrderPacketInfo,
        SortedDictionary<string, object?>? attributes,
        IReadOnlyCollection<long>? attachmentIds,
        IReadOnlyCollection<long>? markingAttachmentIds)
    {
        Code = code.Trim();
        Name = name.Trim();
        Description = description;
        BulkOrderPacketInfo = !string.IsNullOrWhiteSpace(bulkOrderPacketInfo) ? bulkOrderPacketInfo : retailOrderPacketInfo;
        RetailOrderPacketInfo = !string.IsNullOrWhiteSpace(retailOrderPacketInfo) ? retailOrderPacketInfo : bulkOrderPacketInfo;
        ChangeAttributes(attributes);
        AddDomainEvent(new SalesSpuSavedEvent(this, attachmentIds, markingAttachmentIds));
    }

    /// <summary>
    /// 修改价格
    /// </summary>
    /// <param name="price">价格</param>
    public void ModifyPrice(decimal price)
    {
        Price = price;
    }

    /// <summary>
    /// 删除
    /// </summary>
    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new SalesSpuDeletedEvent(this));
    }

    /// <summary>
    /// 获取分配项列表(只读)
    /// </summary>
    /// <returns>只读分配项列表</returns>
    public IReadOnlyList<SalesSpuAssignItem> GetSalesSpuAssignItems()
    {
        return SalesSpuAssignItems.ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取工序列表(只读)
    /// </summary>
    /// <returns>只读工序列表</returns>
    public IReadOnlyList<SalesSpuWorkStep> GetSalesSpuWorkSteps()
    {
        return SalesSpuWorkSteps.ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取路线列表(只读)
    /// </summary>
    /// <returns>只读路线列表</returns>
    public IReadOnlyList<SalesSpuRouting> GetSalesSpuRoutings()
    {
        return SalesSpuRoutings.ToList().AsReadOnly();
    }

    /// <summary>
    /// 新增分配项
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="sequence">序列</param>
    /// <param name="isPriorityCustomized">是否优先订购</param>
    public void AddAssignItem(long spuId, long warehouseId, int sequence, bool? isPriorityCustomized)
    {
        if (IsExistAssignItem(spuId, warehouseId))
        {
            throw new UserFriendlyException("分配项已存在!");
        }

        SalesSpuAssignItems.Add(new SalesSpuAssignItem(spuId, warehouseId, sequence, isPriorityCustomized));
    }

    /// <summary>
    /// 修改分配项
    /// </summary>
    /// <param name="assignItemId">分配项Id</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="sequence">序列</param>
    /// <param name="isPriorityCustomized">是否优先订购</param>
    public void ModifyAssignItem(long assignItemId, long spuId, long warehouseId, int sequence, bool? isPriorityCustomized)
    {
        SalesSpuAssignItem? item = SalesSpuAssignItems.FirstOrDefault(x => x.Id == assignItemId);
        if (item is null)
        {
            throw new UserFriendlyException("分配项不存在!");
        }

        item.Modify(spuId, warehouseId, sequence, isPriorityCustomized);
    }

    /// <summary>
    /// 删除分配项
    /// </summary>
    /// <param name="assignItemId">分配项Id</param>
    public void DeleteAssignItem(long assignItemId)
    {
        SalesSpuAssignItem? item = SalesSpuAssignItems.FirstOrDefault(x => x.Id == assignItemId);
        if (item is null)
        {
            return;
        }

        item.Delete();
    }

    /// <summary>
    /// 新增工序
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    public void AddWorkStep(long workStepId)
    {
        if (IsExistWorkStep(workStepId))
        {
            throw new UserFriendlyException("工序已存在!");
        }

        SalesSpuWorkSteps.Add(new SalesSpuWorkStep(workStepId));
    }

    /// <summary>
    /// 删除工序
    /// </summary>
    /// <param name="salesWorkStepId">销售产品工序Id</param>
    public void DeleteWorkStep(long salesWorkStepId)
    {
        SalesSpuWorkStep? item = SalesSpuWorkSteps.FirstOrDefault(x => x.Id == salesWorkStepId);
        if (item is null)
        {
            return;
        }

        item.Delete();
    }

    /// <summary>
    /// 获取默认分配项(按序列升序取第一个)
    /// </summary>
    /// <returns>分配项</returns>
    public SalesSpuAssignItem GetDefaultAssignItem()
    {
        SalesSpuAssignItem? assignItem = SalesSpuAssignItems.OrderBy(x => x.Sequence).FirstOrDefault();
        if (assignItem is null)
        {
            throw new UserFriendlyException("销售产品" + Code + "分配项不存在!");
        }

        return assignItem;
    }

    /// <summary>
    /// 新增路线
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="routingId">路线Id</param>
    public void AddRouting(long spuId, long routingId)
    {
        if (IsExistRouting(spuId))
        {
            throw new UserFriendlyException("产品路线已存在!");
        }

        SalesSpuRoutings.Add(new SalesSpuRouting(spuId, routingId));
    }

    /// <summary>
    /// 修改路线
    /// </summary>
    /// <param name="salesSpuRoutingId">销售产品路线Id</param>
    /// <param name="routingId">路线Id</param>
    public void ModifyRouting(long salesSpuRoutingId, long routingId)
    {
        SalesSpuRouting? item = FindRouting(salesSpuRoutingId);
        if (item is null)
        {
            throw new UserFriendlyException("产品路线不存在!");
        }

        item.Modify(routingId);
    }

    /// <summary>
    /// 删除路线
    /// </summary>
    /// <param name="salesSpuRoutingId">销售产品路线Id</param>
    public void DeleteRouting(long salesSpuRoutingId)
    {
        SalesSpuRouting? item = FindRouting(salesSpuRoutingId);
        if (item is null)
        {
            return;
        }

        item.Delete();
    }

    /// <summary>
    /// 按产品取路线
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <returns>路线, 不存在返回 null</returns>
    public SalesSpuRouting? GetRoutingBySpuId(long spuId)
    {
        return SalesSpuRoutings.FirstOrDefault(x => x.SpuId == spuId && !x.IsDeleted);
    }

    /// <summary>
    /// 查找路线(含已删除, 供修改/删除定位)
    /// </summary>
    /// <param name="salesSpuRoutingId">销售产品路线Id</param>
    /// <returns>路线, 不存在返回 null</returns>
    private SalesSpuRouting? FindRouting(long salesSpuRoutingId)
    {
        return SalesSpuRoutings.FirstOrDefault(x => x.Id == salesSpuRoutingId);
    }

    /// <summary>
    /// 路线是否已存在(未删除)
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistRouting(long spuId)
    {
        return SalesSpuRoutings.Any(x => x.SpuId == spuId && !x.IsDeleted);
    }

    /// <summary>
    /// 分配项是否已存在(未删除)
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistAssignItem(long spuId, long warehouseId)
    {
        return SalesSpuAssignItems.Any(x => x.SpuId == spuId && x.WarehouseId == warehouseId && !x.IsDeleted);
    }

    /// <summary>
    /// 工序是否已存在(未删除)
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistWorkStep(long workStepId)
    {
        return SalesSpuWorkSteps.Any(x => x.WorkStepId == workStepId && !x.IsDeleted);
    }

    /// <summary>
    /// 校验产品类型
    /// </summary>
    /// <param name="productType">产品类型</param>
    private static void ValidateProductType(string productType)
    {
        if (string.IsNullOrWhiteSpace(productType))
        {
            throw new UserFriendlyException("产品类型不能为空");
        }

        if (!ProductTypeEnum.IsValid(productType))
        {
            throw new UserFriendlyException("产品类型" + productType + "有误!");
        }
    }

    /// <summary>
    /// 变更属性值(自动包装为有序字典)
    /// </summary>
    /// <param name="attributes">属性值</param>
    private void ChangeAttributes(SortedDictionary<string, object?>? attributes)
    {
        Attributes = attributes == null ? null : new SortedDictionary<string, object?>(attributes);
    }
}
