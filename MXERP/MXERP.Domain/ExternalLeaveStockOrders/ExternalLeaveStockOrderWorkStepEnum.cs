namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单工序(对应 Java 的 WorkStepEnum, 编码与名称均为中文)
/// </summary>
public static class ExternalLeaveStockOrderWorkStepEnum
{
    /// <summary>
    /// 送仓库
    /// </summary>
    public const string SongCangKu = "送仓库";

    /// <summary>
    /// 发料
    /// </summary>
    public const string FaLiao = "发料";

    /// <summary>
    /// 扣账
    /// </summary>
    public const string KouZhang = "扣账";

    /// <summary>
    /// 送包装
    /// </summary>
    public const string SongBaoZhuang = "送包装";

    /// <summary>
    /// ETC
    /// </summary>
    public const string Etc = "ETC";

    /// <summary>
    /// 打唛
    /// </summary>
    public const string DaMai = "打唛";

    /// <summary>
    /// 黄油
    /// </summary>
    public const string HuangYou = "黄油";

    /// <summary>
    /// 打印条码
    /// </summary>
    public const string DaYinTiaoMa = "打印条码";

    /// <summary>
    /// 换镜袋
    /// </summary>
    public const string HuanJingDai = "换镜袋";

    /// <summary>
    /// 贴背贴
    /// </summary>
    public const string TieBeiTie = "贴背贴";

    /// <summary>
    /// 打包
    /// </summary>
    public const string DaBao = "打包";

    /// <summary>
    /// 装箱明细
    /// </summary>
    public const string ZhuangXiangMingXi = "装箱明细";

    /// <summary>
    /// 出货检查
    /// </summary>
    public const string ChuHuoJianCha = "出货检查";

    /// <summary>
    /// 封箱
    /// </summary>
    public const string FengXiang = "封箱";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        SongCangKu,
        FaLiao,
        KouZhang,
        SongBaoZhuang,
        Etc,
        DaMai,
        HuangYou,
        DaYinTiaoMa,
        HuanJingDai,
        TieBeiTie,
        DaBao,
        ZhuangXiangMingXi,
        ChuHuoJianCha,
        FengXiang
    };

    /// <summary>
    /// 编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        return code is not null && All.Contains(code);
    }
}
