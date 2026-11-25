namespace MXERP.Domain.Processes;

/// <summary>
/// 工序类型
/// </summary>
public static class ProcessTypeEnum
{
    /// <summary>
    /// 焗色
    /// </summary>
    public const string Coating = "Coating";

    /// <summary>
    /// 茶色
    /// </summary>
    public const string TintingColor = "TintingColor";

    /// <summary>
    /// 水银
    /// </summary>
    public const string Mirror = "Mirror";

    /// <summary>
    /// 染色
    /// </summary>
    public const string TintingCode = "TintingCode";

    /// <summary>
    /// 染色名称
    /// </summary>
    public const string TintingName = "TintingName";

    /// <summary>
    /// 镜架
    /// </summary>
    public const string Frame = "Frame";

    /// <summary>
    /// 彩边
    /// </summary>
    public const string CaiBian = "CaiBian";

    /// <summary>
    /// 车边
    /// </summary>
    public const string CheBian = "CheBian";

    /// <summary>
    /// 开坑
    /// </summary>
    public const string KaiKeng = "KaiKeng";

    /// <summary>
    /// 批花
    /// </summary>
    public const string PiHua = "PiHua";

    /// <summary>
    /// 钻孔
    /// </summary>
    public const string ZuanKong = "ZuanKong";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        Coating,
        TintingColor,
        Mirror,
        TintingCode,
        TintingName,
        Frame,
        CaiBian,
        CheBian,
        KaiKeng,
        PiHua,
        ZuanKong
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
