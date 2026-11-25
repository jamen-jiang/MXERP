using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Roles;

/// <summary>
/// 角色(对应 Java Role)
/// </summary>
public class Role : AuditedAggregateRoot
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private Role()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="remark">备注</param>
    internal Role(string name, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("角色名称不能为空");
        }

        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="remark">备注</param>
    internal void Modify(string name, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("角色名称不能为空");
        }

        Name = name.Trim();
        Remark = remark;
    }
}
