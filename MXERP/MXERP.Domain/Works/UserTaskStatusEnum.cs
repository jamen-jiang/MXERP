namespace MXERP.Domain.Works;

/// <summary>
/// 用户任务状态
/// </summary>
public static class UserTaskStatusEnum
{
    /// <summary>
    /// 同意
    /// </summary>
    public const string Approve = "Approve";

    /// <summary>
    /// 拒绝
    /// </summary>
    public const string Refuse = "Refuse";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Approve,
        Refuse
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
