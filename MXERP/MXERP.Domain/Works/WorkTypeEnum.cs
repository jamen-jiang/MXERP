using System.ComponentModel;

namespace MXERP.Domain.Works;

/// <summary>
/// 工作类型(页面查询用)
/// </summary>
public enum WorkTypeEnum
{
    /// <summary>
    /// 待办
    /// </summary>
    [Description("待办")]
    ToDo,

    /// <summary>
    /// 已办
    /// </summary>
    [Description("已办")]
    Done
}
