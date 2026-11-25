using System.ComponentModel;

namespace MXERP.Domain.Works;

/// <summary>
/// 用户任务状态
/// </summary>
public enum UserTaskStatusEnum
{
    /// <summary>
    /// 同意
    /// </summary>
    [Description("同意")]
    Approve,

    /// <summary>
    /// 拒绝
    /// </summary>
    [Description("拒绝")]
    Refuse
}
