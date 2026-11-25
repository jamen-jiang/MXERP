using Snow.Domain.ValueObjects;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 工序(对应 Java ProcessVO, 作为散镜订单的内嵌值对象)
/// </summary>
public class ProcessVO : ValueObject
{
    /// <summary>
    /// 焗色(可空)
    /// </summary>
    public string? Coating { get; private set; }
    /// <summary>
    /// 茶色(可空)
    /// </summary>
    public string? TintingColor { get; private set; }
    /// <summary>
    /// 水银(可空)
    /// </summary>
    public string? Mirror { get; private set; }
    /// <summary>
    /// 染色(可空)
    /// </summary>
    public string? TintingCode { get; private set; }
    /// <summary>
    /// 染色名称(可空)
    /// </summary>
    public string? TintingName { get; private set; }
    /// <summary>
    /// UV
    /// </summary>
    public bool Uv { get; private set; }
    /// <summary>
    /// 加硬
    /// </summary>
    public bool Hard { get; private set; }
    /// <summary>
    /// 抛光
    /// </summary>
    public bool Polish { get; private set; }
    /// <summary>
    /// 彩边(可空)
    /// </summary>
    public string? Auro { get; private set; }
    /// <summary>
    /// 裁型/车边(可空)
    /// </summary>
    public string? Edge { get; private set; }
    /// <summary>
    /// 开坑(可空)
    /// </summary>
    public string? SuperHole { get; private set; }
    /// <summary>
    /// 批花(可空)
    /// </summary>
    public string? Special { get; private set; }
    /// <summary>
    /// 批花代号(可空)
    /// </summary>
    public string? SpecialCode { get; private set; }
    /// <summary>
    /// 钻孔(可空)
    /// </summary>
    public string? ZuanKong { get; private set; }
    /// <summary>
    /// 镜架(可空)
    /// </summary>
    public string? Frame { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected ProcessVO()
    {
    }

    /// <summary>
    /// 全参构造器
    /// </summary>
    /// <param name="coating">焗色(可空)</param>
    /// <param name="tintingColor">茶色(可空)</param>
    /// <param name="mirror">水银(可空)</param>
    /// <param name="tintingCode">染色(可空)</param>
    /// <param name="tintingName">染色名称(可空)</param>
    /// <param name="uv">UV</param>
    /// <param name="hard">加硬</param>
    /// <param name="polish">抛光</param>
    /// <param name="auro">彩边(可空)</param>
    /// <param name="edge">裁型/车边(可空)</param>
    /// <param name="superHole">开坑(可空)</param>
    /// <param name="special">批花(可空)</param>
    /// <param name="specialCode">批花代号(可空)</param>
    /// <param name="zuanKong">钻孔(可空)</param>
    /// <param name="frame">镜架(可空)</param>
    internal ProcessVO(string? coating, string? tintingColor, string? mirror, string? tintingCode, string? tintingName, bool uv, bool hard, bool polish, string? auro, string? edge, string? superHole, string? special, string? specialCode, string? zuanKong, string? frame)
    {
        Coating = coating;
        TintingColor = tintingColor;
        Mirror = mirror;
        TintingCode = tintingCode;
        TintingName = tintingName;
        Uv = uv;
        Hard = hard;
        Polish = polish;
        Auro = auro;
        Edge = edge;
        SuperHole = superHole;
        Special = special;
        SpecialCode = specialCode;
        ZuanKong = zuanKong;
        Frame = frame;
    }

    /// <summary>
    /// 返回参与判等的属性
    /// </summary>
    /// <returns>属性序列</returns>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        return new object?[]
        {
            Coating, TintingColor, Mirror, TintingCode, TintingName, Uv, Hard, Polish,
            Auro, Edge, SuperHole, Special, SpecialCode, ZuanKong, Frame
        };
    }
}
