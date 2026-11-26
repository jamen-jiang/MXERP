using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.Spus;

/// <summary>
/// 产品
/// </summary>
public class Spu : AuditedAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;

    /// <summary>
    /// 组别
    /// </summary>
    public string? Group { get; private set; }

    /// <summary>
    /// 供应商Id
    /// </summary>
    public long SupplierId { get; private set; }

    /// <summary>
    /// 品牌Id
    /// </summary>
    public long? BrandId { get; private set; }

    /// <summary>
    /// 产品代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 产品描述
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// 采购价格
    /// </summary>
    public decimal PurchasePrice { get; private set; }

    /// <summary>
    /// 币种Id
    /// </summary>
    public long CurrencyId { get; private set; }

    /// <summary>
    /// 单位Id
    /// </summary>
    public long UnitId { get; private set; }

    /// <summary>
    /// 税率Id
    /// </summary>
    public long? TaxRateId { get; private set; }

    /// <summary>
    /// 起订量
    /// </summary>
    public decimal? Moq { get; private set; }

    /// <summary>
    /// 是否禁用
    /// </summary>
    public bool Disabled { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 属性值
    /// </summary>
    public SortedDictionary<string, object?>? Attributes { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected Spu()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="group">组别</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">产品代码</param>
    /// <param name="name">产品名称</param>
    /// <param name="description">产品描述</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="remark">备注</param>
    /// <param name="attributes">属性值</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="attachmentIds">附件Id集合</param>
    internal Spu(
        string productType,
        string? group,
        long supplierId,
        long? brandId,
        string code,
        string name,
        string? description,
        decimal purchasePrice,
        long currencyId,
        long unitId,
        long? taxRateId,
        decimal? moq,
        string? remark,
        SortedDictionary<string, object?>? attributes,
        IReadOnlyCollection<long>? companyIds,
        IReadOnlyCollection<long>? attachmentIds)
    {
        ProductType = productType;
        Group = group;
        SupplierId = supplierId;
        BrandId = brandId;
        Code = code;
        Name = name;
        Description = description;
        PurchasePrice = purchasePrice;
        CurrencyId = currencyId;
        UnitId = unitId;
        TaxRateId = taxRateId;
        Moq = moq;
        Remark = remark;
        ChangeAttributes(attributes);
        AddDomainEvent(new SpuSavedEvent(this, companyIds, attachmentIds));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="group">组别</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">产品代码</param>
    /// <param name="name">产品名称</param>
    /// <param name="description">产品描述</param>
    /// <param name="purchasePrice">采购价格</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="unitId">单位Id</param>
    /// <param name="taxRateId">税率Id</param>
    /// <param name="moq">起订量</param>
    /// <param name="remark">备注</param>
    /// <param name="attributes">属性值</param>
    /// <param name="companyIds">公司Id集合</param>
    /// <param name="attachmentIds">附件Id集合</param>
    public void Modify(
        string? group,
        long supplierId,
        long? brandId,
        string code,
        string name,
        string? description,
        decimal purchasePrice,
        long currencyId,
        long unitId,
        long? taxRateId,
        decimal? moq,
        string? remark,
        SortedDictionary<string, object?>? attributes,
        IReadOnlyCollection<long>? companyIds,
        IReadOnlyCollection<long>? attachmentIds)
    {
        Group = group;
        SupplierId = supplierId;
        BrandId = brandId;
        Code = code;
        Name = name;
        Description = description;
        PurchasePrice = purchasePrice;
        CurrencyId = currencyId;
        UnitId = unitId;
        TaxRateId = taxRateId;
        Moq = moq;
        Remark = remark;
        ChangeAttributes(attributes);
        AddDomainEvent(new SpuSavedEvent(this, companyIds, attachmentIds));
    }

    /// <summary>
    /// 删除
    /// </summary>
    public override void Delete()
    {
        base.Delete();
        AddDomainEvent(new SpuDeletedEvent(this));
    }

    /// <summary>
    /// 启用
    /// </summary>
    public void Enable()
    {
        Disabled = false;
    }

    /// <summary>
    /// 禁用
    /// </summary>
    public void Disable()
    {
        Disabled = true;
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
