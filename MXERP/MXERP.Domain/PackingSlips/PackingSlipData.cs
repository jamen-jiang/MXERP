namespace MXERP.Domain.PackingSlips;

/// <summary>
/// 装箱单数据
/// </summary>
public sealed class PackingSlipData
{
    /// <summary>
    /// 订单号
    /// </summary>
    public string OrderNumber { get; init; } = string.Empty;

    /// <summary>
    /// 客户单号
    /// </summary>
    public string CustomerOrderNumber { get; init; } = string.Empty;

    /// <summary>
    /// 门店代码
    /// </summary>
    public string StoreCode { get; init; } = string.Empty;

    /// <summary>
    /// 下单人
    /// </summary>
    public string? Orderer { get; init; }

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; init; }

    /// <summary>
    /// 焗色
    /// </summary>
    public string Coating { get; init; } = string.Empty;

    /// <summary>
    /// UV
    /// </summary>
    public bool Uv { get; init; } = false;

    /// <summary>
    /// Hard
    /// </summary>
    public bool Hard { get; init; } = false;

    /// <summary>
    /// 销售镜种代码R
    /// </summary>
    public string SalesLensCodeR { get; init; } = string.Empty;

    /// <summary>
    /// 销售镜种名称R
    /// </summary>
    public string SalesLensNameR { get; init; } = string.Empty;

    /// <summary>
    /// 是否RX(R)
    /// </summary>
    public bool IsRxR { get; init; } = false;

    /// <summary>
    /// 镜大R
    /// </summary>
    public int DiameterR { get; init; } = 0;

    /// <summary>
    /// 数量R
    /// </summary>
    public int QtyR { get; init; } = 0;

    /// <summary>
    /// SPHR
    /// </summary>
    public decimal? SphR { get; init; }

    /// <summary>
    /// CYLR
    /// </summary>
    public decimal? CylR { get; init; }

    /// <summary>
    /// 线位R
    /// </summary>
    public int AxisR { get; init; } = 0;

    /// <summary>
    /// AddR
    /// </summary>
    public decimal? AddR { get; init; }

    /// <summary>
    /// Prism1R
    /// </summary>
    public string? Prism1R { get; init; }

    /// <summary>
    /// Base1R
    /// </summary>
    public string? Base1R { get; init; }

    /// <summary>
    /// Prism2R
    /// </summary>
    public string? Prism2R { get; init; }

    /// <summary>
    /// Base2R
    /// </summary>
    public string? Base2R { get; init; }

    /// <summary>
    /// ETR
    /// </summary>
    public string? EtR { get; init; }

    /// <summary>
    /// BCR
    /// </summary>
    public string? BcR { get; init; }

    /// <summary>
    /// DecenterR
    /// </summary>
    public string? DecenterR { get; init; }

    /// <summary>
    /// AngleR
    /// </summary>
    public string? AngleR { get; init; }

    /// <summary>
    /// DistanceR
    /// </summary>
    public string? DistanceR { get; init; }

    /// <summary>
    /// ThicknessR
    /// </summary>
    public string? ThicknessR { get; init; }

    /// <summary>
    /// ColourR
    /// </summary>
    public string? ColourR { get; init; }

    /// <summary>
    /// IntermediateDistanceR
    /// </summary>
    public string? IntermediateDistanceR { get; init; }

    /// <summary>
    /// ReadingDistanceR
    /// </summary>
    public string? ReadingDistanceR { get; init; }

    /// <summary>
    /// VertexDistanceR
    /// </summary>
    public string? VertexDistanceR { get; init; }

    /// <summary>
    /// WrapAngleR
    /// </summary>
    public string? WrapAngleR { get; init; }

    /// <summary>
    /// TiltAngleR
    /// </summary>
    public string? TiltAngleR { get; init; }

    /// <summary>
    /// PDR
    /// </summary>
    public decimal? PdR { get; init; }

    /// <summary>
    /// EyePointR
    /// </summary>
    public string? EyePointR { get; init; }

    /// <summary>
    /// FreamDataBoxAR
    /// </summary>
    public string? FreamDataBoxAR { get; init; }

    /// <summary>
    /// FreamDataBoxBR
    /// </summary>
    public string? FreamDataBoxBR { get; init; }

    /// <summary>
    /// FreamDataBoxEDR
    /// </summary>
    public string? FreamDataBoxEDR { get; init; }

    /// <summary>
    /// DBLR
    /// </summary>
    public decimal? DblR { get; init; }

    /// <summary>
    /// MirrorCoatingR
    /// </summary>
    public string? MirrorCoatingR { get; init; }

    /// <summary>
    /// 销售镜种代码L
    /// </summary>
    public string SalesLensCodeL { get; init; } = string.Empty;

    /// <summary>
    /// 销售镜种名称L
    /// </summary>
    public string SalesLensNameL { get; init; } = string.Empty;

    /// <summary>
    /// 是否RX(L)
    /// </summary>
    public bool IsRxL { get; init; } = false;

    /// <summary>
    /// 镜大L
    /// </summary>
    public int DiameterL { get; init; } = 0;

    /// <summary>
    /// 数量L
    /// </summary>
    public int QtyL { get; init; } = 0;

    /// <summary>
    /// SPHL
    /// </summary>
    public decimal? SphL { get; init; }

    /// <summary>
    /// CYLL
    /// </summary>
    public decimal? CylL { get; init; }

    /// <summary>
    /// 线位L
    /// </summary>
    public int AxisL { get; init; } = 0;

    /// <summary>
    /// AddL
    /// </summary>
    public decimal? AddL { get; init; }

    /// <summary>
    /// Prism1L
    /// </summary>
    public string? Prism1L { get; init; }

    /// <summary>
    /// Base1L
    /// </summary>
    public string? Base1L { get; init; }

    /// <summary>
    /// Prism2L
    /// </summary>
    public string? Prism2L { get; init; }

    /// <summary>
    /// Base2L
    /// </summary>
    public string? Base2L { get; init; }

    /// <summary>
    /// ETL
    /// </summary>
    public string? EtL { get; init; }

    /// <summary>
    /// BCL
    /// </summary>
    public string? BcL { get; init; }

    /// <summary>
    /// DecenterL
    /// </summary>
    public string? DecenterL { get; init; }

    /// <summary>
    /// AngleL
    /// </summary>
    public string? AngleL { get; init; }

    /// <summary>
    /// DistanceL
    /// </summary>
    public string? DistanceL { get; init; }

    /// <summary>
    /// ThicknessL
    /// </summary>
    public string? ThicknessL { get; init; }

    /// <summary>
    /// ColourL
    /// </summary>
    public string? ColourL { get; init; }

    /// <summary>
    /// IntermediateDistanceL
    /// </summary>
    public string? IntermediateDistanceL { get; init; }

    /// <summary>
    /// ReadingDistanceL
    /// </summary>
    public string? ReadingDistanceL { get; init; }

    /// <summary>
    /// VertexDistanceL
    /// </summary>
    public string? VertexDistanceL { get; init; }

    /// <summary>
    /// WrapAngleL
    /// </summary>
    public string? WrapAngleL { get; init; }

    /// <summary>
    /// TiltAngleL
    /// </summary>
    public string? TiltAngleL { get; init; }

    /// <summary>
    /// PDL
    /// </summary>
    public decimal? PdL { get; init; }

    /// <summary>
    /// EyePointL
    /// </summary>
    public string? EyePointL { get; init; }

    /// <summary>
    /// FreamDataBoxAL
    /// </summary>
    public string? FreamDataBoxAL { get; init; }

    /// <summary>
    /// FreamDataBoxBL
    /// </summary>
    public string? FreamDataBoxBL { get; init; }

    /// <summary>
    /// FreamDataBoxEDL
    /// </summary>
    public string? FreamDataBoxEDL { get; init; }

    /// <summary>
    /// DBLL
    /// </summary>
    public decimal? DblL { get; init; }

    /// <summary>
    /// MirrorCoatingL
    /// </summary>
    public string? MirrorCoatingL { get; init; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; init; }
}
