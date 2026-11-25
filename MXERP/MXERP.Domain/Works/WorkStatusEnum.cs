namespace MXERP.Domain.Works;

/// <summary>
/// 工作状态
/// </summary>
public static class WorkStatusEnum
{
    /// <summary>
    /// 运行中
    /// </summary>
    public const string Running = "Running";

    /// <summary>
    /// 已完成
    /// </summary>
    public const string Finished = "Finished";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        Running,
        Finished
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
