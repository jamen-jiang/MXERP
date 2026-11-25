using MXERP.Domain.Common.Enums;

using Snow.Domain.Entities.Auditing;
using Snow.Domain.Events;

namespace MXERP.Domain.CustomersSpus;

/// <summary>
/// 客人镜种
/// </summary>
public class CustomerSpu : AuditedAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public ProductTypeEnum ProductType { get; private set; }

    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long SalesSpuId { get; private set; }

    /// <summary>
    /// 品牌Id
    /// </summary>
    public long BrandId { get; private set; }

    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 镜袋编号（可空）
    /// </summary>
    public string? BagNumber { get; private set; }

    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected CustomerSpu()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="bagNumber">镜袋编号（可空）</param>
    internal CustomerSpu(ProductTypeEnum productType, long salesSpuId, long brandId, string code, string name, string? bagNumber)
    {
        ProductType = productType;
        SalesSpuId = salesSpuId;
        BrandId = brandId;
        Code = code.Trim();
        Name = name.Trim();
        BagNumber = bagNumber;
    }

    /// <summary>
    /// 构造器（带附件，触发领域事件）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="bagNumber">镜袋编号（可空）</param>
    /// <param name="attachmentIds">附件Id集合（可空）</param>
    internal CustomerSpu(ProductTypeEnum productType, long salesSpuId, long brandId, string code, string name, string? bagNumber, IReadOnlyCollection<long>? attachmentIds)
        : this(productType, salesSpuId, brandId, code, name, bagNumber)
    {
        AddDomainEvent(new CustomerSpuSavedEvent(this, attachmentIds));
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="bagNumber">镜袋编号（可空）</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="brandId">品牌Id</param>
    internal void Modify(string name, string? bagNumber, long salesSpuId, long brandId)
    {
        Name = name.Trim();
        BagNumber = bagNumber;
        SalesSpuId = salesSpuId;
        BrandId = brandId;
    }

    /// <summary>
    /// 修改（带附件，触发领域事件）
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="bagNumber">镜袋编号（可空）</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="brandId">品牌Id</param>
    /// <param name="attachmentIds">附件Id集合（可空）</param>
    public void Modify(string name, string? bagNumber, long salesSpuId, long brandId, IReadOnlyCollection<long>? attachmentIds)
    {
        Modify(name, bagNumber, salesSpuId, brandId);
        AddDomainEvent(new CustomerSpuSavedEvent(this, attachmentIds));
    }
}
