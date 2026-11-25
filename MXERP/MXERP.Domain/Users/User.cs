using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Users;

/// <summary>
/// 用户(对应 Java User)
/// </summary>
public class User : AuditedAggregateRoot
{
    /// <summary>
    /// 用户名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 账号
    /// </summary>
    public string UserName { get; private set; } = string.Empty;
    /// <summary>
    /// 邮箱(可空)
    /// </summary>
    public string? Email { get; private set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; private set; } = string.Empty;
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 用户岗位列表
    /// </summary>
    private readonly List<UserPost> _userPosts;

    /// <summary>
    /// 用户岗位列表
    /// </summary>
    public IReadOnlyCollection<UserPost> UserPosts
    {
        get { return _userPosts.AsReadOnly(); }
    }

    /// <summary>
    /// 用户角色列表
    /// </summary>
    private readonly List<UserRole> _userRoles;

    /// <summary>
    /// 用户角色列表
    /// </summary>
    public IReadOnlyCollection<UserRole> UserRoles
    {
        get { return _userRoles.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected User()
    {
        _userPosts = new List<UserPost>();
        _userRoles = new List<UserRole>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="name">用户名称</param>
    /// <param name="userName">账号</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="password">密码</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="postIds">岗位Id列表</param>
    /// <param name="roleIds">角色Id列表(可空)</param>
    internal User(string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds)
        : this()
    {
        Validate(name, userName, password, postIds);

        Name = name.Trim();
        UserName = userName.Trim();
        Email = email?.Trim();
        Password = password;
        Remark = remark;
        SyncPosts(postIds);
        SyncRoles(roleIds);
    }

    /// <summary>
    /// 修改用户
    /// </summary>
    /// <param name="name">用户名称</param>
    /// <param name="userName">账号</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="password">密码</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="postIds">岗位Id列表</param>
    /// <param name="roleIds">角色Id列表(可空)</param>
    internal void Modify(string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds)
    {
        Validate(name, userName, password, postIds);

        Name = name.Trim();
        UserName = userName.Trim();
        Email = email?.Trim();
        Password = password;
        Remark = remark;
        SyncPosts(postIds);
        SyncRoles(roleIds);
    }

    /// <summary>
    /// 校验必填项
    /// </summary>
    /// <param name="name">用户名称</param>
    /// <param name="userName">账号</param>
    /// <param name="password">密码</param>
    /// <param name="postIds">岗位Id列表</param>
    private static void Validate(string name, string userName, string password, List<long> postIds)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("用户名称不能为空");
        }
        if (userName.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("账号不能为空");
        }
        if (password.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("密码不能为空");
        }
        if (postIds.Count == 0)
        {
            throw new UserFriendlyException("岗位不允许为空!");
        }
    }

    /// <summary>
    /// 同步岗位列表
    /// </summary>
    /// <param name="postIds">岗位Id列表</param>
    private void SyncPosts(List<long> postIds)
    {
        List<UserPost> toRemoves = _userPosts
            .Where(a => postIds.All(i => i != a.PostId))
            .ToList();
        foreach (UserPost item in toRemoves)
        {
            DeletePost(item.Id);
        }

        foreach (long postId in postIds)
        {
            bool exists = _userPosts.Any(x => x.PostId == postId && !x.IsDeleted);
            if (!exists)
            {
                _userPosts.Add(new UserPost(postId));
            }
        }
    }

    /// <summary>
    /// 同步角色列表
    /// </summary>
    /// <param name="roleIds">角色Id列表(可空)</param>
    private void SyncRoles(List<long>? roleIds)
    {
        List<long> target = roleIds ?? new List<long>();

        List<UserRole> toRemoves = _userRoles
            .Where(a => target.All(i => i != a.RoleId))
            .ToList();
        foreach (UserRole item in toRemoves)
        {
            DeleteRole(item.Id);
        }

        foreach (long roleId in target)
        {
            bool exists = _userRoles.Any(x => x.RoleId == roleId && !x.IsDeleted);
            if (!exists)
            {
                _userRoles.Add(new UserRole(roleId));
            }
        }
    }

    /// <summary>
    /// 删除岗位(用户岗位关联)
    /// </summary>
    /// <param name="userPostId">用户岗位关联Id</param>
    private void DeletePost(long userPostId)
    {
        UserPost? item = _userPosts.FirstOrDefault(x => x.Id == userPostId);
        if (item is not null)
        {
            item.Delete();
        }
    }

    /// <summary>
    /// 删除角色(用户角色关联)
    /// </summary>
    /// <param name="userRoleId">用户角色关联Id</param>
    private void DeleteRole(long userRoleId)
    {
        UserRole? item = _userRoles.FirstOrDefault(x => x.Id == userRoleId);
        if (item is not null)
        {
            item.Delete();
        }
    }
}
