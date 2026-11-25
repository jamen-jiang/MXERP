using System.ComponentModel;

namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 眼别(对应 Java EyeTypeEnum, 以编码字符串存储)
/// </summary>
public enum EyeTypeEnum
{
    /// <summary>
    /// 双眼
    /// </summary>
    [Description("双眼")]
    RL,

    /// <summary>
    /// 右眼
    /// </summary>
    [Description("右眼")]
    R,

    /// <summary>
    /// 左眼
    /// </summary>
    [Description("左眼")]
    L
}
