using System.ComponentModel;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单工序(对应 Java 的 WorkStepEnum, 编码与名称均为中文)
/// </summary>
public enum ExternalLeaveStockOrderWorkStepEnum
{
    /// <summary>
    /// 送仓库
    /// </summary>
    [Description("送仓库")]
    SongCangKu,

    /// <summary>
    /// 发料
    /// </summary>
    [Description("发料")]
    FaLiao,

    /// <summary>
    /// 扣账
    /// </summary>
    [Description("扣账")]
    KouZhang,

    /// <summary>
    /// 送包装
    /// </summary>
    [Description("送包装")]
    SongBaoZhuang,

    /// <summary>
    /// ETC
    /// </summary>
    [Description("ETC")]
    Etc,

    /// <summary>
    /// 打唛
    /// </summary>
    [Description("打唛")]
    DaMai,

    /// <summary>
    /// 黄油
    /// </summary>
    [Description("黄油")]
    HuangYou,

    /// <summary>
    /// 打印条码
    /// </summary>
    [Description("打印条码")]
    DaYinTiaoMa,

    /// <summary>
    /// 换镜袋
    /// </summary>
    [Description("换镜袋")]
    HuanJingDai,

    /// <summary>
    /// 贴背贴
    /// </summary>
    [Description("贴背贴")]
    TieBeiTie,

    /// <summary>
    /// 打包
    /// </summary>
    [Description("打包")]
    DaBao,

    /// <summary>
    /// 装箱明细
    /// </summary>
    [Description("装箱明细")]
    ZhuangXiangMingXi,

    /// <summary>
    /// 出货检查
    /// </summary>
    [Description("出货检查")]
    ChuHuoJianCha,

    /// <summary>
    /// 封箱
    /// </summary>
    [Description("封箱")]
    FengXiang
}
