using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Users;

/// <summary>
/// 用户绑定的角色(对应 Java User_Role)
/// </summary>
public class UserRole : DeletionAuditedEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    public long? UserId { get; private set; }
    /// <summary>
    /// 角色Id
    /// </summary>
    public long RoleId { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected UserRole()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="roleId">角色Id</param>
    public UserRole(long roleId)
    {
        RoleId = roleId;
    }
}
