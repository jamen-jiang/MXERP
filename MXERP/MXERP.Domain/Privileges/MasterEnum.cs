namespace MXERP.Domain.Privileges;

/// <summary>
/// 权限对象(对应 Java 的 MasterEnum, 以编码字符串存储)
/// </summary>
public static class MasterEnum
{
    /// <summary>
    /// 角色
    /// </summary>
    public const string Role = "Role";
    /// <summary>
    /// 用户
    /// </summary>
    public const string User = "User";
    /// <summary>
    /// 部门
    /// </summary>
    public const string Department = "Department";
    /// <summary>
    /// 岗位
    /// </summary>
    public const string Post = "Post";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Role, User, Department, Post
    };

    /// <summary>
    /// 编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        return code is not null && All.Contains(code);
    }
}
