using System.ComponentModel;

namespace MXERP.Domain.Spus;

/// <summary>
/// 组别
/// </summary>
public enum SpuGroupEnum
{
    /// <summary>
    /// 常用
    /// </summary>
    [Description("常用")]
    Common,

    /// <summary>
    /// 不常用
    /// </summary>
    [Description("不常用")]
    Unusual,

    /// <summary>
    /// 呆滞
    /// </summary>
    [Description("呆滞")]
    Dull,

    /// <summary>
    /// 新增
    /// </summary>
    [Description("新增")]
    New
}
