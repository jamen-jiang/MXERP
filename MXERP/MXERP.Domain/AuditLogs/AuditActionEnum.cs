namespace MXERP.Domain.AuditLogs;

/// <summary>
/// 审核动作(对应 Java 的 AuditActionEnum, 以编码字符串存储)
/// </summary>
public static class AuditActionEnum
{
    /// <summary>
    /// 修改
    /// </summary>
    public const string Modify = "Modify";
    /// <summary>
    /// 打印
    /// </summary>
    public const string Print = "Print";
    /// <summary>
    /// 提交
    /// </summary>
    public const string Commit = "Commit";
    /// <summary>
    /// 弃提
    /// </summary>
    public const string UnCommit = "UnCommit";
    /// <summary>
    /// 拒审
    /// </summary>
    public const string RejectCheck = "RejectCheck";
    /// <summary>
    /// 审核
    /// </summary>
    public const string Check = "Check";
    /// <summary>
    /// 弃审
    /// </summary>
    public const string UnCheck = "UnCheck";
    /// <summary>
    /// 拒通
    /// </summary>
    public const string RejectPass = "RejectPass";
    /// <summary>
    /// 通过
    /// </summary>
    public const string Pass = "Pass";
    /// <summary>
    /// 弃通
    /// </summary>
    public const string UnPass = "UnPass";
    /// <summary>
    /// 拒完
    /// </summary>
    public const string RejectFinish = "RejectFinish";
    /// <summary>
    /// 完成
    /// </summary>
    public const string Finish = "Finish";
    /// <summary>
    /// 弃完
    /// </summary>
    public const string UnFinish = "UnFinish";
    /// <summary>
    /// 取消
    /// </summary>
    public const string Cancel = "Cancel";
    /// <summary>
    /// 删除
    /// </summary>
    public const string Delete = "Delete";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Modify, Print, Commit, UnCommit, RejectCheck, Check, UnCheck, RejectPass, Pass, UnPass,
        RejectFinish, Finish, UnFinish, Cancel, Delete
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
