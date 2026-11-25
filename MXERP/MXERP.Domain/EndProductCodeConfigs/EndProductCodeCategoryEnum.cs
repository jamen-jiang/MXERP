using System.ComponentModel;

namespace MXERP.Domain.EndProductCodeConfigs;

/// <summary>
/// 成品代码分类
/// </summary>
public enum EndProductCodeCategoryEnum
{
    /// <summary>
    /// 总计
    /// </summary>
    [Description("总计")]
    Active,

    /// <summary>
    /// 停用
    /// </summary>
    [Description("停用")]
    Inactive
}
