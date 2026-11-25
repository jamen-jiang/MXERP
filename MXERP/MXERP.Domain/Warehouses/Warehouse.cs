using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Warehouses;

/// <summary>
/// 仓库(对应 Java Warehouse)
/// </summary>
public class Warehouse : AuditedAggregateRoot
{
    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; private set; }
    /// <summary>
    /// 管理员
    /// </summary>
    public string? ManageManName { get; private set; }
    /// <summary>
    /// 电话
    /// </summary>
    public string? Telephone { get; private set; }
    /// <summary>
    /// 是否默认
    /// </summary>
    public bool IsDefault { get; private set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private Warehouse()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="address">地址</param>
    /// <param name="manageManName">管理员</param>
    /// <param name="telephone">电话</param>
    /// <param name="remark">备注</param>
    internal Warehouse(string code, string name, string? address, string? manageManName, string? telephone, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓库代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓库名称不能为空");
        }

        Code = code.Trim();
        Name = name.Trim();
        Address = address;
        ManageManName = manageManName;
        Telephone = telephone;
        Remark = remark;
    }

    /// <summary>
    /// 修改仓库
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="address">地址</param>
    /// <param name="manageManName">管理员</param>
    /// <param name="telephone">电话</param>
    /// <param name="remark">备注</param>
    public void Modify(string name, string? address, string? manageManName, string? telephone, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓库名称不能为空");
        }

        Name = name.Trim();
        Address = address;
        ManageManName = manageManName;
        Telephone = telephone;
        Remark = remark;
    }

    /// <summary>
    /// 设为默认仓库
    /// </summary>
    internal void MarkAsDefault()
    {
        IsDefault = true;
    }

    /// <summary>
    /// 取消默认仓库
    /// </summary>
    internal void UnmarkDefault()
    {
        IsDefault = false;
    }
}
