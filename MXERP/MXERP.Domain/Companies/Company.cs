using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Companies;

/// <summary>
/// 公司(对应 Java Company)
/// </summary>
public class Company : AuditedAggregateRoot
{
    /// <summary>
    /// 区域
    /// </summary>
    public string Region { get; private set; } = string.Empty;
    /// <summary>
    /// 组别(可空)
    /// </summary>
    public string? Group { get; private set; }
    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 简称(可空)
    /// </summary>
    public string? ShortName { get; private set; }
    /// <summary>
    /// 数字代码(可空)
    /// </summary>
    public string? NumberCode { get; private set; }
    /// <summary>
    /// 电话号码(可空)
    /// </summary>
    public string? Telephone { get; private set; }
    /// <summary>
    /// 传真(可空)
    /// </summary>
    public string? Fax { get; private set; }
    /// <summary>
    /// 邮箱(可空)
    /// </summary>
    public string? Email { get; private set; }
    /// <summary>
    /// 地区代码(可空)
    /// </summary>
    public string? AreaCode { get; private set; }
    /// <summary>
    /// 邮政编码(可空)
    /// </summary>
    public string? PostCode { get; private set; }
    /// <summary>
    /// 币种Id
    /// </summary>
    public long CurrencyId { get; private set; }
    /// <summary>
    /// 加成
    /// </summary>
    public double MarkUp { get; private set; }
    /// <summary>
    /// 地址(可空)
    /// </summary>
    public string? Address { get; private set; }
    /// <summary>
    /// 账户地址(可空)
    /// </summary>
    public string? BillingAddress { get; private set; }
    /// <summary>
    /// 排序(可空)
    /// </summary>
    public int? Sort { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 联系人列表
    /// </summary>
    public virtual ICollection<CompanyContact> CompanyContacts { get; private set; } = new List<CompanyContact>();

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Company()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称(可空)</param>
    /// <param name="numberCode">数字代码(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="address">地址(可空)</param>
    /// <param name="billingAddress">账户地址(可空)</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="remark">备注(可空)</param>
    internal Company(string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("代码不能为空");
        }

        ValidateRegionAndGroup(region?.Trim(), group?.Trim());

        Region = region!.Trim();
        Group = group?.Trim();
        Code = code.Trim();
        Name = name?.Trim() ?? string.Empty;
        ShortName = shortName?.Trim();
        NumberCode = numberCode?.Trim();
        Telephone = telephone;
        Fax = fax;
        Email = email;
        AreaCode = areaCode;
        PostCode = postCode;
        CurrencyId = currencyId;
        Address = address;
        BillingAddress = billingAddress;
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改公司
    /// </summary>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称(可空)</param>
    /// <param name="numberCode">数字代码(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="address">地址(可空)</param>
    /// <param name="billingAddress">账户地址(可空)</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("代码不能为空");
        }

        ValidateRegionAndGroup(region?.Trim(), group?.Trim());

        Region = region!.Trim();
        Group = group?.Trim();
        Code = code.Trim();
        Name = name?.Trim() ?? string.Empty;
        ShortName = shortName?.Trim();
        NumberCode = numberCode?.Trim();
        Telephone = telephone;
        Fax = fax;
        Email = email;
        AreaCode = areaCode;
        PostCode = postCode;
        CurrencyId = currencyId;
        Address = address;
        BillingAddress = billingAddress;
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改加成
    /// </summary>
    /// <param name="markUp">加成</param>
    public void Modify(double markUp)
    {
        MarkUp = markUp;
    }

    /// <summary>
    /// 获取联系人列表(只读)
    /// </summary>
    /// <returns>只读联系人列表</returns>
    public IReadOnlyList<CompanyContact> GetCompanyContacts()
    {
        return CompanyContacts.ToList().AsReadOnly();
    }

    /// <summary>
    /// 添加联系人
    /// </summary>
    /// <param name="name">姓名</param>
    /// <param name="telephone">电话(可空)</param>
    /// <param name="area">地区(可空)</param>
    /// <param name="address">地址(可空)</param>
    public void AddContact(string name, string? telephone, string? area, string? address)
    {
        if (IsExistContact(name))
        {
            throw new UserFriendlyException($"联系人{name}已存在!");
        }

        CompanyContacts.Add(new CompanyContact(name, telephone, area, address));
    }

    /// <summary>
    /// 修改联系人
    /// </summary>
    /// <param name="contactId">联系人Id</param>
    /// <param name="telephone">电话(可空)</param>
    /// <param name="area">地区(可空)</param>
    /// <param name="address">地址(可空)</param>
    public void ModifyContact(long contactId, string? telephone, string? area, string? address)
    {
        CompanyContact? contact = CompanyContacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is null)
        {
            throw new UserFriendlyException($"联系人Id{contactId}不存在!");
        }

        contact.Modify(telephone, area, address);
    }

    /// <summary>
    /// 删除联系人
    /// </summary>
    /// <param name="contactId">联系人Id</param>
    public void DeleteContact(long contactId)
    {
        CompanyContact? contact = CompanyContacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is null)
        {
            return;
        }

        contact.Delete();
    }

    /// <summary>
    /// 校验区域和组别
    /// </summary>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    private static void ValidateRegionAndGroup(string? region, string? group)
    {
        if (!CompanyRegionEnum.IsValid(region))
        {
            throw new UserFriendlyException($"区域{region}有误!");
        }

        if (!string.IsNullOrWhiteSpace(group) && !CompanyGroupEnum.IsValid(group))
        {
            throw new UserFriendlyException($"组别{group}有误!");
        }
    }

    /// <summary>
    /// 判断联系人是否存在(未删除)
    /// </summary>
    /// <param name="name">姓名</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistContact(string name)
    {
        return CompanyContacts.Any(c => c.Name == name && !c.IsDeleted);
    }
}
