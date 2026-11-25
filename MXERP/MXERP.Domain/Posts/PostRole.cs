using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Posts;

/// <summary>
/// 岗位绑定的角色(对应 Java Post_Role)
/// </summary>
public class PostRole : DeletionAuditedEntity
{
    /// <summary>
    /// 岗位Id
    /// </summary>
    public long? PostId { get; private set; }
    /// <summary>
    /// 角色Id
    /// </summary>
    public long RoleId { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected PostRole()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="roleId">角色Id</param>
    public PostRole(long roleId)
    {
        RoleId = roleId;
    }
}
