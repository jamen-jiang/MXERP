using System.ComponentModel;

namespace MXERP.Domain.Privileges;

/// <summary>
/// 权限对象(对应 Java 的 MasterEnum, 以编码字符串存储)
/// </summary>
public enum MasterEnum
{
    /// <summary>
    /// 角色
    /// </summary>
    [Description("角色")]
    Role,

    /// <summary>
    /// 用户
    /// </summary>
    [Description("用户")]
    User,

    /// <summary>
    /// 部门
    /// </summary>
    [Description("部门")]
    Department,

    /// <summary>
    /// 岗位
    /// </summary>
    [Description("岗位")]
    Post
}
