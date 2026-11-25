using System.ComponentModel;

namespace MXERP.Domain.Apps;

/// <summary>
/// 应用类型(对应 Java 的 AppTypeEnum, 以编码字符串存储)
/// </summary>
public enum AppTypeEnum
{
    /// <summary>
    /// 目录
    /// </summary>
    [Description("目录")]
    Catalog,

    /// <summary>
    /// 应用
    /// </summary>
    [Description("应用")]
    App,

    /// <summary>
    /// 资源
    /// </summary>
    [Description("资源")]
    Resource
}
