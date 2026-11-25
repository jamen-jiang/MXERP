using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.PackingSlips;

/// <summary>
/// 装箱单
/// </summary>
public class PackingSlip : AuditedAggregateRoot
{
    /// <summary>
    /// 二维码
    /// </summary>
    public string QrCode { get; private set; } = string.Empty;

    /// <summary>
    /// 箱号
    /// </summary>
    public int CartonNo { get; private set; }

    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderNumber { get; private set; } = string.Empty;

    /// <summary>
    /// 客户单号
    /// </summary>
    public string CustomerOrderNumber { get; private set; } = string.Empty;

    /// <summary>
    /// 门店Id
    /// </summary>
    public long StoreId { get; private set; }

    /// <summary>
    /// 下单人
    /// </summary>
    public string? Orderer { get; private set; }

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; private set; }

    /// <summary>
    /// 焗色
    /// </summary>
    public string Coating { get; private set; } = string.Empty;

    /// <summary>
    /// UV
    /// </summary>
    public bool Uv { get; private set; } = false;

    /// <summary>
    /// Hard
    /// </summary>
    public bool Hard { get; private set; } = false;

    /// <summary>
    /// 销售镜种代码R
    /// </summary>
    public string SalesLensCodeR { get; private set; } = string.Empty;

    /// <summary>
    /// 销售镜种名称R
    /// </summary>
    public string SalesLensNameR { get; private set; } = string.Empty;

    /// <summary>
    /// 是否RX(R)
    /// </summary>
    public bool IsRxR { get; private set; } = false;

    /// <summary>
    /// 镜大R
    /// </summary>
    public int DiameterR { get; private set; } = 0;

    /// <summary>
    /// 数量R
    /// </summary>
    public int QtyR { get; private set; } = 0;

    /// <summary>
    /// SPHR
    /// </summary>
    public decimal? SphR { get; private set; }

    /// <summary>
    /// CYLR
    /// </summary>
    public decimal? CylR { get; private set; }

    /// <summary>
    /// 线位R
    /// </summary>
    public int AxisR { get; private set; } = 0;

    /// <summary>
    /// AddR
    /// </summary>
    public decimal? AddR { get; private set; }

    /// <summary>
    /// Prism1R
    /// </summary>
    public string? Prism1R { get; private set; }

    /// <summary>
    /// Base1R
    /// </summary>
    public string? Base1R { get; private set; }

    /// <summary>
    /// Prism2R
    /// </summary>
    public string? Prism2R { get; private set; }

    /// <summary>
    /// Base2R
    /// </summary>
    public string? Base2R { get; private set; }

    /// <summary>
    /// ETR
    /// </summary>
    public string? EtR { get; private set; }

    /// <summary>
    /// BCR
    /// </summary>
    public string? BcR { get; private set; }

    /// <summary>
    /// DecenterR
    /// </summary>
    public string? DecenterR { get; private set; }

    /// <summary>
    /// AngleR
    /// </summary>
    public string? AngleR { get; private set; }

    /// <summary>
    /// DistanceR
    /// </summary>
    public string? DistanceR { get; private set; }

    /// <summary>
    /// ThicknessR
    /// </summary>
    public string? ThicknessR { get; private set; }

    /// <summary>
    /// ColourR
    /// </summary>
    public string? ColourR { get; private set; }

    /// <summary>
    /// IntermediateDistanceR
    /// </summary>
    public string? IntermediateDistanceR { get; private set; }

    /// <summary>
    /// ReadingDistanceR
    /// </summary>
    public string? ReadingDistanceR { get; private set; }

    /// <summary>
    /// VertexDistanceR
    /// </summary>
    public string? VertexDistanceR { get; private set; }

    /// <summary>
    /// WrapAngleR
    /// </summary>
    public string? WrapAngleR { get; private set; }

    /// <summary>
    /// TiltAngleR
    /// </summary>
    public string? TiltAngleR { get; private set; }

    /// <summary>
    /// PDR
    /// </summary>
    public decimal? PdR { get; private set; }

    /// <summary>
    /// EyePointR
    /// </summary>
    public string? EyePointR { get; private set; }

    /// <summary>
    /// FreamDataBoxAR
    /// </summary>
    public string? FreamDataBoxAR { get; private set; }

    /// <summary>
    /// FreamDataBoxBR
    /// </summary>
    public string? FreamDataBoxBR { get; private set; }

    /// <summary>
    /// FreamDataBoxEDR
    /// </summary>
    public string? FreamDataBoxEDR { get; private set; }

    /// <summary>
    /// DBLR
    /// </summary>
    public decimal? DblR { get; private set; }

    /// <summary>
    /// MirrorCoatingR
    /// </summary>
    public string? MirrorCoatingR { get; private set; }

    /// <summary>
    /// 销售镜种代码L
    /// </summary>
    public string SalesLensCodeL { get; private set; } = string.Empty;

    /// <summary>
    /// 销售镜种名称L
    /// </summary>
    public string SalesLensNameL { get; private set; } = string.Empty;

    /// <summary>
    /// 是否RX(L)
    /// </summary>
    public bool IsRxL { get; private set; } = false;

    /// <summary>
    /// 镜大L
    /// </summary>
    public int DiameterL { get; private set; } = 0;

    /// <summary>
    /// 数量L
    /// </summary>
    public int QtyL { get; private set; } = 0;

    /// <summary>
    /// SPHL
    /// </summary>
    public decimal? SphL { get; private set; }

    /// <summary>
    /// CYLL
    /// </summary>
    public decimal? CylL { get; private set; }

    /// <summary>
    /// 线位L
    /// </summary>
    public int AxisL { get; private set; } = 0;

    /// <summary>
    /// AddL
    /// </summary>
    public decimal? AddL { get; private set; }

    /// <summary>
    /// Prism1L
    /// </summary>
    public string? Prism1L { get; private set; }

    /// <summary>
    /// Base1L
    /// </summary>
    public string? Base1L { get; private set; }

    /// <summary>
    /// Prism2L
    /// </summary>
    public string? Prism2L { get; private set; }

    /// <summary>
    /// Base2L
    /// </summary>
    public string? Base2L { get; private set; }

    /// <summary>
    /// ETL
    /// </summary>
    public string? EtL { get; private set; }

    /// <summary>
    /// BCL
    /// </summary>
    public string? BcL { get; private set; }

    /// <summary>
    /// DecenterL
    /// </summary>
    public string? DecenterL { get; private set; }

    /// <summary>
    /// AngleL
    /// </summary>
    public string? AngleL { get; private set; }

    /// <summary>
    /// DistanceL
    /// </summary>
    public string? DistanceL { get; private set; }

    /// <summary>
    /// ThicknessL
    /// </summary>
    public string? ThicknessL { get; private set; }

    /// <summary>
    /// ColourL
    /// </summary>
    public string? ColourL { get; private set; }

    /// <summary>
    /// IntermediateDistanceL
    /// </summary>
    public string? IntermediateDistanceL { get; private set; }

    /// <summary>
    /// ReadingDistanceL
    /// </summary>
    public string? ReadingDistanceL { get; private set; }

    /// <summary>
    /// VertexDistanceL
    /// </summary>
    public string? VertexDistanceL { get; private set; }

    /// <summary>
    /// WrapAngleL
    /// </summary>
    public string? WrapAngleL { get; private set; }

    /// <summary>
    /// TiltAngleL
    /// </summary>
    public string? TiltAngleL { get; private set; }

    /// <summary>
    /// PDL
    /// </summary>
    public decimal? PdL { get; private set; }

    /// <summary>
    /// EyePointL
    /// </summary>
    public string? EyePointL { get; private set; }

    /// <summary>
    /// FreamDataBoxAL
    /// </summary>
    public string? FreamDataBoxAL { get; private set; }

    /// <summary>
    /// FreamDataBoxBL
    /// </summary>
    public string? FreamDataBoxBL { get; private set; }

    /// <summary>
    /// FreamDataBoxEDL
    /// </summary>
    public string? FreamDataBoxEDL { get; private set; }

    /// <summary>
    /// DBLL
    /// </summary>
    public decimal? DblL { get; private set; }

    /// <summary>
    /// MirrorCoatingL
    /// </summary>
    public string? MirrorCoatingL { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 出货时间
    /// </summary>
    public DateTime DeliveredOn { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected PackingSlip()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="qrCode">二维码</param>
    /// <param name="cartonNo">箱号</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="data">装箱单数据</param>
    internal PackingSlip(string qrCode, int cartonNo, long storeId, PackingSlipData data)
    {
        QrCode = qrCode;
        CartonNo = cartonNo;
        OrderNumber = data.OrderNumber;
        CustomerOrderNumber = data.CustomerOrderNumber;
        StoreId = storeId;
        Orderer = data.Orderer;
        Brand = data.Brand;
        Coating = data.Coating;
        Uv = data.Uv;
        Hard = data.Hard;
        SalesLensCodeR = data.SalesLensCodeR;
        SalesLensNameR = data.SalesLensNameR;
        IsRxR = data.IsRxR;
        DiameterR = data.DiameterR;
        QtyR = data.QtyR;
        SphR = data.SphR;
        CylR = data.CylR;
        AxisR = data.AxisR;
        AddR = data.AddR;
        Prism1R = data.Prism1R;
        Base1R = data.Base1R;
        Prism2R = data.Prism2R;
        Base2R = data.Base2R;
        EtR = data.EtR;
        BcR = data.BcR;
        DecenterR = data.DecenterR;
        AngleR = data.AngleR;
        DistanceR = data.DistanceR;
        ThicknessR = data.ThicknessR;
        ColourR = data.ColourR;
        IntermediateDistanceR = data.IntermediateDistanceR;
        ReadingDistanceR = data.ReadingDistanceR;
        VertexDistanceR = data.VertexDistanceR;
        WrapAngleR = data.WrapAngleR;
        TiltAngleR = data.TiltAngleR;
        PdR = data.PdR;
        EyePointR = data.EyePointR;
        FreamDataBoxAR = data.FreamDataBoxAR;
        FreamDataBoxBR = data.FreamDataBoxBR;
        FreamDataBoxEDR = data.FreamDataBoxEDR;
        DblR = data.DblR;
        MirrorCoatingR = data.MirrorCoatingR;
        SalesLensCodeL = data.SalesLensCodeL;
        SalesLensNameL = data.SalesLensNameL;
        IsRxL = data.IsRxL;
        DiameterL = data.DiameterL;
        QtyL = data.QtyL;
        SphL = data.SphL;
        CylL = data.CylL;
        AxisL = data.AxisL;
        AddL = data.AddL;
        Prism1L = data.Prism1L;
        Base1L = data.Base1L;
        Prism2L = data.Prism2L;
        Base2L = data.Base2L;
        EtL = data.EtL;
        BcL = data.BcL;
        DecenterL = data.DecenterL;
        AngleL = data.AngleL;
        DistanceL = data.DistanceL;
        ThicknessL = data.ThicknessL;
        ColourL = data.ColourL;
        IntermediateDistanceL = data.IntermediateDistanceL;
        ReadingDistanceL = data.ReadingDistanceL;
        VertexDistanceL = data.VertexDistanceL;
        WrapAngleL = data.WrapAngleL;
        TiltAngleL = data.TiltAngleL;
        PdL = data.PdL;
        EyePointL = data.EyePointL;
        FreamDataBoxAL = data.FreamDataBoxAL;
        FreamDataBoxBL = data.FreamDataBoxBL;
        FreamDataBoxEDL = data.FreamDataBoxEDL;
        DblL = data.DblL;
        MirrorCoatingL = data.MirrorCoatingL;
        Remark = data.Remark;
        DeliveredOn = DateTime.Now;
        if (DeliveredOn.DayOfWeek == DayOfWeek.Saturday)
        {
            DeliveredOn = DeliveredOn.AddDays(1);
        }
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="data">装箱单数据</param>
    internal void Modify(PackingSlipData data)
    {
        Orderer = data.Orderer;
        Brand = data.Brand;
        Coating = data.Coating;
        Uv = data.Uv;
        Hard = data.Hard;
        SalesLensCodeR = data.SalesLensCodeR;
        SalesLensNameR = data.SalesLensNameR;
        IsRxR = data.IsRxR;
        DiameterR = data.DiameterR;
        QtyR = data.QtyR;
        SphR = data.SphR;
        CylR = data.CylR;
        AxisR = data.AxisR;
        AddR = data.AddR;
        Prism1R = data.Prism1R;
        Base1R = data.Base1R;
        Prism2R = data.Prism2R;
        Base2R = data.Base2R;
        EtR = data.EtR;
        BcR = data.BcR;
        DecenterR = data.DecenterR;
        AngleR = data.AngleR;
        DistanceR = data.DistanceR;
        ThicknessR = data.ThicknessR;
        ColourR = data.ColourR;
        IntermediateDistanceR = data.IntermediateDistanceR;
        ReadingDistanceR = data.ReadingDistanceR;
        VertexDistanceR = data.VertexDistanceR;
        WrapAngleR = data.WrapAngleR;
        TiltAngleR = data.TiltAngleR;
        PdR = data.PdR;
        EyePointR = data.EyePointR;
        FreamDataBoxAR = data.FreamDataBoxAR;
        FreamDataBoxBR = data.FreamDataBoxBR;
        FreamDataBoxEDR = data.FreamDataBoxEDR;
        DblR = data.DblR;
        MirrorCoatingR = data.MirrorCoatingR;
        SalesLensCodeL = data.SalesLensCodeL;
        SalesLensNameL = data.SalesLensNameL;
        IsRxL = data.IsRxL;
        DiameterL = data.DiameterL;
        QtyL = data.QtyL;
        SphL = data.SphL;
        CylL = data.CylL;
        AxisL = data.AxisL;
        AddL = data.AddL;
        Prism1L = data.Prism1L;
        Base1L = data.Base1L;
        Prism2L = data.Prism2L;
        Base2L = data.Base2L;
        EtL = data.EtL;
        BcL = data.BcL;
        DecenterL = data.DecenterL;
        AngleL = data.AngleL;
        DistanceL = data.DistanceL;
        ThicknessL = data.ThicknessL;
        ColourL = data.ColourL;
        IntermediateDistanceL = data.IntermediateDistanceL;
        ReadingDistanceL = data.ReadingDistanceL;
        VertexDistanceL = data.VertexDistanceL;
        WrapAngleL = data.WrapAngleL;
        TiltAngleL = data.TiltAngleL;
        PdL = data.PdL;
        EyePointL = data.EyePointL;
        FreamDataBoxAL = data.FreamDataBoxAL;
        FreamDataBoxBL = data.FreamDataBoxBL;
        FreamDataBoxEDL = data.FreamDataBoxEDL;
        DblL = data.DblL;
        MirrorCoatingL = data.MirrorCoatingL;
        Remark = data.Remark;
    }

    /// <summary>
    /// 修改出货时间
    /// </summary>
    /// <param name="deliveredOn">出货时间</param>
    internal void Modify(DateTime deliveredOn)
    {
        DeliveredOn = deliveredOn;
    }
}
