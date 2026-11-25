using System.ComponentModel;

namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 日期类型(对应 Java 的 DateTypeEnum, 以编码字符串存储)
/// </summary>
public enum DateTypeEnum
{
    /// <summary>
    /// 年
    /// </summary>
    [Description("年")]
    Year,

    /// <summary>
    /// 月
    /// </summary>
    [Description("月")]
    Month,

    /// <summary>
    /// 日
    /// </summary>
    [Description("日")]
    Day
}
