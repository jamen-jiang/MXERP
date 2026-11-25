using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Suppliers;

/// <summary>
/// 供应商(对应 Java Supplier, 继承 UpdateAggregateRoot)
/// </summary>
public class Supplier : AuditedAggregateRoot
{
    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 旧代码
    /// </summary>
    public string? OldCode { get; private set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 编号
    /// </summary>
    public string? Number { get; private set; }
    /// <summary>
    /// 简称
    /// </summary>
    public string? ShortName { get; private set; }
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; private set; }
    /// <summary>
    /// 电话
    /// </summary>
    public string? Telephone { get; private set; }
    /// <summary>
    /// 传真
    /// </summary>
    public string? Fax { get; private set; }
    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; private set; }
    /// <summary>
    /// 联系人
    /// </summary>
    public string? ContactPerson { get; private set; }
    /// <summary>
    /// 税率Id
    /// </summary>
    public long? TaxRateId { get; private set; }
    /// <summary>
    /// 付款方式
    /// </summary>
    public string? PayMode { get; private set; }
    /// <summary>
    /// 交货方式
    /// </summary>
    public string? DeliveryMode { get; private set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private Supplier()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    internal Supplier(
        string code,
        string? oldCode,
        string name,
        string? number,
        string? shortName,
        string? address,
        string? telephone,
        string? fax,
        string? email,
        string? contactPerson,
        long? taxRateId,
        string? payMode,
        string? deliveryMode,
        string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("供应商代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("供应商名称不能为空");
        }

        Code = code.Trim();
        OldCode = oldCode;
        Name = name.Trim();
        Number = number;
        ShortName = shortName;
        Address = address;
        Telephone = telephone;
        Fax = fax;
        Email = email;
        ContactPerson = contactPerson;
        TaxRateId = taxRateId;
        PayMode = payMode;
        DeliveryMode = deliveryMode;
        Remark = remark;
    }

    /// <summary>
    /// 修改供应商
    /// </summary>
    internal void Modify(
        string code,
        string? oldCode,
        string name,
        string? number,
        string? shortName,
        string? address,
        string? telephone,
        string? fax,
        string? email,
        string? contactPerson,
        long? taxRateId,
        string? payMode,
        string? deliveryMode,
        string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("供应商代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("供应商名称不能为空");
        }

        Code = code.Trim();
        OldCode = oldCode;
        Name = name.Trim();
        Number = number;
        ShortName = shortName;
        Address = address;
        Telephone = telephone;
        Fax = fax;
        Email = email;
        ContactPerson = contactPerson;
        TaxRateId = taxRateId;
        PayMode = payMode;
        DeliveryMode = deliveryMode;
        Remark = remark;
    }
}
