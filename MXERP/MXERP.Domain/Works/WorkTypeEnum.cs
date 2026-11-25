namespace MXERP.Domain.Works;

/// <summary>
/// 工作类型(页面查询用)
/// </summary>
public static class WorkTypeEnum
{
    /// <summary>
    /// 待办
    /// </summary>
    public const string ToDo = "ToDo";

    /// <summary>
    /// 已办
    /// </summary>
    public const string Done = "Done";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        ToDo,
        Done
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>true 表示有效</returns>
    public static bool IsValid(string? code)
    {
        if (code is null)
        {
            return false;
        }

        return All.Contains(code);
    }
}
