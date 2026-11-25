namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单工序(对应 Java RetailOrderWorkStepEnum, 以编码字符串存储)
/// </summary>
public static class RetailOrderWorkStepEnum
{
    /// <summary>
    /// 接受订单
    /// </summary>
    public const string JieShouDingDan = "JieShouDingDan";
    /// <summary>
    /// 送仓库
    /// </summary>
    public const string SongCangKu = "SongCangKu";
    /// <summary>
    /// 送包装
    /// </summary>
    public const string SongBaoZhuang = "SongBaoZhuang";
    /// <summary>
    /// 出货
    /// </summary>
    public const string ChuHuo = "ChuHuo";
    /// <summary>
    /// 取消
    /// </summary>
    public const string QuXiao = "QuXiao";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        JieShouDingDan, SongCangKu, SongBaoZhuang, ChuHuo, QuXiao
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
