using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Stores;

/// <summary>
/// 门店联系人(对应 Java StoreContact)
/// </summary>
public class StoreContact : DeletionAuditedEntity
{
    /// <summary>
    /// 门店Id
    /// </summary>
    public long? StoreId { get; private set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 电话(可空)
    /// </summary>
    public string? Telephone { get; private set; }
    /// <summary>
    /// 地区(可空)
    /// </summary>
    public string? Area { get; private set; }
    /// <summary>
    /// 地址(可空)
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected StoreContact()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="name">姓名</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="telephone">电话(可空)</param>
    /// <param name="area">地区(可空)</param>
    /// <param name="address">地址(可空)</param>
    internal StoreContact(string name, long companyId, string? telephone, string? area, string? address)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("姓名不能为空");
        }

        Name = name.Trim();
        CompanyId = companyId;
        Telephone = telephone;
        Area = area;
        Address = address;
    }

    /// <summary>
    /// 修改联系人
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="telephone">电话(可空)</param>
    /// <param name="area">地区(可空)</param>
    /// <param name="address">地址(可空)</param>
    internal void Modify(long companyId, string? telephone, string? area, string? address)
    {
        CompanyId = companyId;
        Telephone = telephone;
        Area = area;
        Address = address;
    }
}
