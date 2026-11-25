using System.ComponentModel;

namespace MXERP.Domain.Works;

/// <summary>
/// 任务
/// </summary>
public enum TaskEnum
{
    /// <summary>
    /// 审核
    /// </summary>
    [Description("审核")]
    Check,

    /// <summary>
    /// 批准
    /// </summary>
    [Description("批准")]
    Approve
}
