using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Posts;

/// <summary>
/// 岗位(对应 Java Post)
/// </summary>
public class Post : AuditedAggregateRoot
{
    /// <summary>
    /// 岗位名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; private set; }
    /// <summary>
    /// 职级Id
    /// </summary>
    public long RankId { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 岗位角色列表
    /// </summary>
    public virtual ICollection<PostRole> PostRoles { get; private set; } = new List<PostRole>();

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Post()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="name">岗位名称</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="rankId">职级Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="roleIds">角色Id列表</param>
    internal Post(string name, long departmentId, long rankId, string? remark, List<long> roleIds)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("岗位名称不能为空");
        }

        Name = name.Trim();
        DepartmentId = departmentId;
        RankId = rankId;
        Remark = remark;
        SyncRoles(roleIds);
    }

    /// <summary>
    /// 修改岗位
    /// </summary>
    /// <param name="name">岗位名称</param>
    /// <param name="departmentId">部门Id</param>
    /// <param name="rankId">职级Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="roleIds">角色Id列表</param>
    internal void Modify(string name, long departmentId, long rankId, string? remark, List<long> roleIds)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("岗位名称不能为空");
        }

        Name = name.Trim();
        DepartmentId = departmentId;
        RankId = rankId;
        Remark = remark;
        SyncRoles(roleIds);
    }

    /// <summary>
    /// 获取岗位角色列表(只读)
    /// </summary>
    /// <returns>只读角色列表</returns>
    public IReadOnlyList<PostRole> GetPostRoles()
    {
        return PostRoles.ToList().AsReadOnly();
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    /// <param name="roleId">角色Id</param>
    internal void DeleteRole(long roleId)
    {
        PostRole? item = PostRoles.FirstOrDefault(x => x.Id == roleId && !x.IsDeleted);
        if (item is not null)
        {
            item.Delete();
        }
    }

    /// <summary>
    /// 同步角色列表
    /// </summary>
    /// <param name="roleIds">角色Id列表</param>
    private void SyncRoles(List<long> roleIds)
    {
        List<PostRole> toRemoves = PostRoles
            .Where(a => roleIds.All(i => i != a.RoleId))
            .ToList();
        foreach (PostRole item in toRemoves)
        {
            DeleteRole(item.Id);
        }

        foreach (long roleId in roleIds)
        {
            if (PostRoles.All(x => x.RoleId != roleId))
            {
                PostRoles.Add(new PostRole(roleId));
            }
        }
    }
}
