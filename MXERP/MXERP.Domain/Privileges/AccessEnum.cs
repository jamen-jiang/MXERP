using System.ComponentModel;

namespace MXERP.Domain.Privileges;

/// <summary>
/// 权限通道(对应 Java 的 AccessEnum, 以编码字符串存储)
/// </summary>
public enum AccessEnum
{
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
