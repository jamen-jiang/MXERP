using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Departments;

/// <summary>
/// 部门(对应 Java Department, 继承 UpdateAggregateRoot)
/// </summary>
public class Department : AuditedAggregateRoot
{
    /// <summary>
    /// 层级路径
    /// </summary>
    public string? Level { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 上级Id
    /// </summary>
    public long? ParentId { get; private set; }
    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 数字编码
    /// </summary>
    public string? NumberCode { get; private set; }
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
    /// 排序
    /// </summary>
    public int? Sort { get; private set; }
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; private set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private Department()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    internal Department(
        string? level,
        long companyId,
        long? parentId,
        string code,
        string name,
        string? numberCode,
        string? telephone,
        string? fax,
        string? email,
        int? sort,
        string? address,
        string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("部门代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("部门名称不能为空");
        }

        Level = level?.Trim();
        CompanyId = companyId;
        ParentId = parentId;
        Code = code.Trim();
        Name = name.Trim();
        NumberCode = numberCode?.Trim();
        Telephone = telephone;
        Fax = fax;
        Email = email;
        Sort = sort;
        Address = address;
        Remark = remark;
    }

    /// <summary>
    /// 修改部门
    /// </summary>
    public void Modify(
        string? level,
        long companyId,
        long? parentId,
        string code,
        string name,
        string? numberCode,
        string? telephone,
        string? fax,
        string? email,
        int? sort,
        string? address,
        string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("部门代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("部门名称不能为空");
        }

        Level = level?.Trim();
        CompanyId = companyId;
        ParentId = parentId;
        Code = code.Trim();
        Name = name.Trim();
        NumberCode = numberCode?.Trim();
        Telephone = telephone;
        Fax = fax;
        Email = email;
        Sort = sort;
        Address = address;
        Remark = remark;
    }

    /// <summary>
    /// ChangeLevel(部门)
    /// </summary>
    /// <param name="level">层级路径</param>
    public void ChangeLevel(string level)
    {
        Level = level;
    }
}
