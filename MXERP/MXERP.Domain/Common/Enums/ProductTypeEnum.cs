using System.ComponentModel;

namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 产品类型枚举
/// </summary>
public enum ProductTypeEnum
{
    /// <summary>
    /// 成品
    /// </summary>
    [Description("成品")]
    EndProduct,

    /// <summary>
    /// 半成品
    /// </summary>
    [Description("半成品")]
    SemiProduct,

    /// <summary>
    /// 辅料
    /// </summary>
    [Description("辅料")]
    Accessory
}
