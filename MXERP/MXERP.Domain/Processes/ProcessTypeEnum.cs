using System.ComponentModel;

namespace MXERP.Domain.Processes;

/// <summary>
/// 工序类型
/// </summary>
public enum ProcessTypeEnum
{
    /// <summary>
    /// 焗色
    /// </summary>
    [Description("焗色")]
    Coating,

    /// <summary>
    /// 茶色
    /// </summary>
    [Description("茶色")]
    TintingColor,

    /// <summary>
    /// 水银
    /// </summary>
    [Description("水银")]
    Mirror,

    /// <summary>
    /// 染色
    /// </summary>
    [Description("染色")]
    TintingCode,

    /// <summary>
    /// 染色名称
    /// </summary>
    [Description("染色名称")]
    TintingName,

    /// <summary>
    /// 镜架
    /// </summary>
    [Description("镜架")]
    Frame,

    /// <summary>
    /// 彩边
    /// </summary>
    [Description("彩边")]
    CaiBian,

    /// <summary>
    /// 车边
    /// </summary>
    [Description("车边")]
    CheBian,

    /// <summary>
    /// 开坑
    /// </summary>
    [Description("开坑")]
    KaiKeng,

    /// <summary>
    /// 批花
    /// </summary>
    [Description("批花")]
    PiHua,

    /// <summary>
    /// 钻孔
    /// </summary>
    [Description("钻孔")]
    ZuanKong
}
