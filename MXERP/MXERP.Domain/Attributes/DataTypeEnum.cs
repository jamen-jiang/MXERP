using System.ComponentModel;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 数据类型
/// </summary>
public enum DataTypeEnum
{
    /// <summary>
    /// 字符串
    /// </summary>
    [Description("字符串")]
    String,

    /// <summary>
    /// 整数
    /// </summary>
    [Description("整数")]
    Integer,

    /// <summary>
    /// 小数
    /// </summary>
    [Description("小数")]
    Decimal,

    /// <summary>
    /// 布尔值
    /// </summary>
    [Description("布尔值")]
    Boolean,

    /// <summary>
    /// 日期
    /// </summary>
    [Description("日期")]
    DateTime
}
