using System.ComponentModel;

namespace MXERP.Domain.Companies;

/// <summary>
/// 公司组别(对应 Java 的 CompanyGroupEnum, 以编码字符串存储)
/// </summary>
public enum CompanyGroupEnum
{
    /// <summary>
    /// 海外
    /// </summary>
    [Description("海外")]
    Overseas,

    /// <summary>
    /// 东南亚
    /// </summary>
    [Description("东南亚")]
    Sea,

    /// <summary>
    /// 其他分工厂
    /// </summary>
    [Description("其他分工厂")]
    Other,

    /// <summary>
    /// 国内
    /// </summary>
    [Description("国内")]
    Domestic
}
