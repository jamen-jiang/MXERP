using System.ComponentModel;

namespace MXERP.Domain.Companies;

/// <summary>
/// 公司区域(对应 Java 的 CompanyRegionEnum, 以编码字符串存储)
/// </summary>
public enum CompanyRegionEnum
{
    /// <summary>
    /// 国内
    /// </summary>
    [Description("国内")]
    Domestic,

    /// <summary>
    /// 国际
    /// </summary>
    [Description("国际")]
    International
}
