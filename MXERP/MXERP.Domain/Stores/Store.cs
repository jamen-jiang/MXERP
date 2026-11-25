using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Stores;

/// <summary>
/// 门店(对应 Java Store)
/// </summary>
public class Store : AuditedAggregateRoot
{
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 门店Code
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 门店名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 门店简称(可空)
    /// </summary>
    public string? ShortName { get; private set; }
    /// <summary>
    /// 门店编号(可空)
    /// </summary>
    public string? Number { get; private set; }
    /// <summary>
    /// 发票抬头(可空)
    /// </summary>
    public string? InvoiceTitle { get; private set; }
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
    /// 区号代码(可空)
    /// </summary>
    public string? CityCode { get; private set; }
    /// <summary>
    /// 区号名称(可空)
    /// </summary>
    public string? CityName { get; private set; }
    /// <summary>
    /// 地区代码(可空)
    /// </summary>
    public string? AreaCode { get; private set; }
    /// <summary>
    /// 邮政编码(可空)
    /// </summary>
    public string? PostCode { get; private set; }
    /// <summary>
    /// 物流代码(可空)
    /// </summary>
    public string? LogisticsCode { get; private set; }
    /// <summary>
    /// 物流名称(可空)
    /// </summary>
    public string? LogisticsName { get; private set; }
    /// <summary>
    /// 代表人(可空)
    /// </summary>
    public string? Signer { get; private set; }
    /// <summary>
    /// 门店转换代码(可空)
    /// </summary>
    public string? ConvertCode { get; private set; }
    /// <summary>
    /// 门店地址(可空)
    /// </summary>
    public string? Address { get; private set; }
    /// <summary>
    /// 门店账户地址(可空)
    /// </summary>
    public string? BillingAddress { get; private set; }

    /// <summary>
    /// 联系人列表
    /// </summary>
    private readonly List<StoreContact> _storeContacts;

    /// <summary>
    /// 联系人列表
    /// </summary>
    public IReadOnlyCollection<StoreContact> StoreContacts
    {
        get { return _storeContacts.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Store()
    {
        _storeContacts = new List<StoreContact>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">门店代码</param>
    /// <param name="name">门店名称</param>
    /// <param name="shortName">门店简称(可空)</param>
    /// <param name="number">门店编号(可空)</param>
    /// <param name="invoiceTitle">发票抬头(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="cityCode">区号代码(可空)</param>
    /// <param name="cityName">区号名称(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="logisticsCode">物流代码(可空)</param>
    /// <param name="logisticsName">物流名称(可空)</param>
    /// <param name="signer">代表人(可空)</param>
    /// <param name="convertCode">门店转换代码(可空)</param>
    /// <param name="address">门店地址(可空)</param>
    /// <param name="billingAddress">门店账户地址(可空)</param>
    internal Store(long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress)
        : this()
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("门店代码不能为空");
        }
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("门店名称不能为空");
        }

        CompanyId = companyId;
        Code = code.Trim();
        Name = name.Trim();
        ShortName = shortName;
        Number = number;
        InvoiceTitle = invoiceTitle;
        Telephone = telephone;
        Fax = fax;
        Email = email;
        CityCode = cityCode;
        CityName = cityName;
        AreaCode = areaCode;
        PostCode = postCode;
        LogisticsCode = logisticsCode;
        LogisticsName = logisticsName;
        Signer = signer;
        ConvertCode = convertCode;
        Address = address;
        BillingAddress = billingAddress;
    }

    /// <summary>
    /// 修改门店
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="code">门店代码</param>
    /// <param name="name">门店名称</param>
    /// <param name="shortName">门店简称(可空)</param>
    /// <param name="number">门店编号(可空)</param>
    /// <param name="invoiceTitle">发票抬头(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="cityCode">区号代码(可空)</param>
    /// <param name="cityName">区号名称(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="logisticsCode">物流代码(可空)</param>
    /// <param name="logisticsName">物流名称(可空)</param>
    /// <param name="signer">代表人(可空)</param>
    /// <param name="convertCode">门店转换代码(可空)</param>
    /// <param name="address">门店地址(可空)</param>
    /// <param name="billingAddress">门店账户地址(可空)</param>
    internal void Modify(long companyId, string code, string name, string? shortName, string? number, string? invoiceTitle, string? telephone, string? fax, string? email, string? cityCode, string? cityName, string? areaCode, string? postCode, string? logisticsCode, string? logisticsName, string? signer, string? convertCode, string? address, string? billingAddress)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("门店代码不能为空");
        }
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("门店名称不能为空");
        }

        CompanyId = companyId;
        Code = code.Trim();
        Name = name.Trim();
        ShortName = shortName;
        Number = number;
        InvoiceTitle = invoiceTitle;
        Telephone = telephone;
        Fax = fax;
        Email = email;
        CityCode = cityCode;
        CityName = cityName;
        AreaCode = areaCode;
        PostCode = postCode;
        LogisticsCode = logisticsCode;
        LogisticsName = logisticsName;
        Signer = signer;
        ConvertCode = convertCode;
        Address = address;
        BillingAddress = billingAddress;
    }

    /// <summary>
    /// 添加联系人
    /// </summary>
    /// <param name="name">姓名</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="telephone">电话(可空)</param>
    /// <param name="area">地区(可空)</param>
    /// <param name="address">地址(可空)</param>
    public void AddContact(string name, long companyId, string? telephone, string? area, string? address)
    {
        if (IsExistContact(name))
        {
            throw new UserFriendlyException($"联系人{name}已存在!");
        }

        _storeContacts.Add(new StoreContact(name, companyId, telephone, area, address));
    }

    /// <summary>
    /// 修改联系人
    /// </summary>
    /// <param name="contactId">联系人Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="telephone">电话(可空)</param>
    /// <param name="area">地区(可空)</param>
    /// <param name="address">地址(可空)</param>
    public void ModifyContact(long contactId, long companyId, string? telephone, string? area, string? address)
    {
        StoreContact? contact = _storeContacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is null)
        {
            throw new UserFriendlyException($"联系人Id{contactId}不存在!");
        }

        contact.Modify(companyId, telephone, area, address);
    }

    /// <summary>
    /// 删除联系人
    /// </summary>
    /// <param name="contactId">联系人Id</param>
    public void DeleteContact(long contactId)
    {
        StoreContact? contact = _storeContacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is null)
        {
            return;
        }

        contact.Delete();
    }

    /// <summary>
    /// 判断联系人是否存在(未删除)
    /// </summary>
    /// <param name="name">姓名</param>
    /// <returns>存在返回 true</returns>
    private bool IsExistContact(string name)
    {
        return _storeContacts.Any(c => c.Name == name && !c.IsDeleted);
    }
}
