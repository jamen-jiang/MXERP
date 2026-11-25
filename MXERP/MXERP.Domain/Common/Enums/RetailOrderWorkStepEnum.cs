namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 散镜订单工序
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
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        JieShouDingDan,
        SongCangKu,
        SongBaoZhuang,
        ChuHuo,
        QuXiao
    };

    /// <summary>
    /// 判断编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>true 表示有效</returns>
    public static bool IsValid(string? code)
    {
        if (code is null)
        {
            return false;
        }

        return All.Contains(code);
    }
}
