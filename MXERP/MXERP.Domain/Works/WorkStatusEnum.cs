using System.ComponentModel;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作状态
/// </summary>
public enum WorkStatusEnum
{
    /// <summary>
    /// 运行中
    /// </summary>
    [Description("运行中")]
    Running,

    /// <summary>
    /// 已完成
    /// </summary>
    [Description("已完成")]
    Finished
}
