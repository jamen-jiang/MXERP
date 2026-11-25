using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.SerialNumbers;

namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票(对应 Java TripleInvoice)
/// </summary>
public class TripleInvoice : OrderAggregateRoot
{
    /// <summary>
    /// 客户抬头发票号
    /// </summary>
    public string CustomerInvoiceTitleNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 散镜订单Id
    /// </summary>
    public long RetailOrderId { get; private set; }
    /// <summary>
    /// 散镜订单号
    /// </summary>
    public string RetailOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 散镜订单客户单号
    /// </summary>
    public string RetailOrderCustomerOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 散镜订单客户PO号(可空)
    /// </summary>
    public string? RetailOrderCustomerPONumber { get; private set; }
    /// <summary>
    /// 门店Id
    /// </summary>
    public long StoreId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 门店代码
    /// </summary>
    public string StoreCode { get; private set; } = string.Empty;
    /// <summary>
    /// 客户编号
    /// </summary>
    public string StoreNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 客户发票抬头
    /// </summary>
    public string StoreInvoiceTitle { get; private set; } = string.Empty;
    /// <summary>
    /// 客户地址(可空)
    /// </summary>
    public string? StoreAddress { get; private set; }
    /// <summary>
    /// 客户电话(可空)
    /// </summary>
    public string? StoreTelephone { get; private set; }

    /// <summary>
    /// 销售产品IdR
    /// </summary>
    public long SalesSpuIdR { get; private set; }
    /// <summary>
    /// 销售产品CodeR
    /// </summary>
    public string SalesSpuCodeR { get; private set; } = string.Empty;
    /// <summary>
    /// 客人产品IdR(可空)
    /// </summary>
    public long? CustomerSpuIdR { get; private set; }
    /// <summary>
    /// 客人产品CodeR(可空)
    /// </summary>
    public string? CustomerSpuCodeR { get; private set; }
    /// <summary>
    /// 产品IdR
    /// </summary>
    public long SpuIdR { get; private set; }
    /// <summary>
    /// 仓库IdR
    /// </summary>
    public long WarehouseIdR { get; private set; }
    /// <summary>
    /// 度数类型R
    /// </summary>
    public string DegreesTypeR { get; private set; } = string.Empty;
    /// <summary>
    /// SPHR
    /// </summary>
    public int Sphr { get; private set; }
    /// <summary>
    /// CYLR
    /// </summary>
    public int Cylr { get; private set; }
    /// <summary>
    /// ADDR(可空)
    /// </summary>
    public int? Addr { get; private set; }
    /// <summary>
    /// 直径R(可空)
    /// </summary>
    public int? DiameterR { get; private set; }
    /// <summary>
    /// 轴位R(可空)
    /// </summary>
    public int? Axisr { get; private set; }
    /// <summary>
    /// 瞳距R(可空)
    /// </summary>
    public double? Pdr { get; private set; }
    /// <summary>
    /// 镜高R(可空)
    /// </summary>
    public double? Phr { get; private set; }
    /// <summary>
    /// 面弯R(可空)
    /// </summary>
    public string? BaseR { get; private set; }
    /// <summary>
    /// 中心厚度R(可空)
    /// </summary>
    public double? CenterThicknessR { get; private set; }
    /// <summary>
    /// 刀边R(可空)
    /// </summary>
    public bool? KnifeSideR { get; private set; }
    /// <summary>
    /// 移心R(可空)
    /// </summary>
    public string? DecenterR { get; private set; }
    /// <summary>
    /// 棱镜R(可空)
    /// </summary>
    public string? PrismR { get; private set; }
    /// <summary>
    /// 数量R
    /// </summary>
    public double QtyR { get; private set; }

    /// <summary>
    /// 销售产品IdL
    /// </summary>
    public long SalesSpuIdL { get; private set; }
    /// <summary>
    /// 销售产品CodeL
    /// </summary>
    public string SalesSpuCodeL { get; private set; } = string.Empty;
    /// <summary>
    /// 客人产品IdL(可空)
    /// </summary>
    public long? CustomerSpuIdL { get; private set; }
    /// <summary>
    /// 客人产品CodeL(可空)
    /// </summary>
    public string? CustomerSpuCodeL { get; private set; }
    /// <summary>
    /// 产品IdL
    /// </summary>
    public long SpuIdL { get; private set; }
    /// <summary>
    /// 仓库IdL
    /// </summary>
    public long WarehouseIdL { get; private set; }
    /// <summary>
    /// 度数类型L
    /// </summary>
    public string DegreesTypeL { get; private set; } = string.Empty;
    /// <summary>
    /// SPHL
    /// </summary>
    public int Sphl { get; private set; }
    /// <summary>
    /// CYLL
    /// </summary>
    public int Cyll { get; private set; }
    /// <summary>
    /// ADDL(可空)
    /// </summary>
    public int? Addl { get; private set; }
    /// <summary>
    /// 直径L(可空)
    /// </summary>
    public int? DiameterL { get; private set; }
    /// <summary>
    /// 轴位L(可空)
    /// </summary>
    public int? Axisl { get; private set; }
    /// <summary>
    /// 瞳距L(可空)
    /// </summary>
    public double? Pdl { get; private set; }
    /// <summary>
    /// 镜高L(可空)
    /// </summary>
    public double? Phl { get; private set; }
    /// <summary>
    /// 面弯L(可空)
    /// </summary>
    public string? BaseL { get; private set; }
    /// <summary>
    /// 中心厚度L(可空)
    /// </summary>
    public double? CenterThicknessL { get; private set; }
    /// <summary>
    /// 刀边L(可空)
    /// </summary>
    public bool? KnifeSideL { get; private set; }
    /// <summary>
    /// 移心L(可空)
    /// </summary>
    public string? DecenterL { get; private set; }
    /// <summary>
    /// 棱镜L(可空)
    /// </summary>
    public string? PrismL { get; private set; }
    /// <summary>
    /// 数量L
    /// </summary>
    public double QtyL { get; private set; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected TripleInvoice()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="data">三联发票数据</param>
    internal TripleInvoice(TripleInvoiceData data)
    {
        CustomerInvoiceTitleNumber = data.CustomerInvoiceTitleNumber;
        RetailOrderId = data.RetailOrderId;
        RetailOrderNumber = data.RetailOrderNumber;
        RetailOrderCustomerOrderNumber = data.RetailOrderCustomerOrderNumber;
        RetailOrderCustomerPONumber = data.RetailOrderCustomerPONumber;
        StoreId = data.StoreId;
        CompanyId = data.CompanyId;
        StoreCode = data.StoreCode;
        StoreNumber = data.StoreNumber;
        StoreInvoiceTitle = data.StoreInvoiceTitle;
        StoreAddress = data.StoreAddress;
        StoreTelephone = data.StoreTelephone;

        SalesSpuIdR = data.SalesSpuIdR;
        SalesSpuCodeR = data.SalesSpuCodeR;
        CustomerSpuIdR = data.CustomerSpuIdR;
        CustomerSpuCodeR = data.CustomerSpuCodeR;
        SpuIdR = data.SpuIdR;
        WarehouseIdR = data.WarehouseIdR;
        DegreesTypeR = data.DegreesTypeR;
        Sphr = data.Sphr;
        Cylr = data.Cylr;
        Addr = data.Addr;
        DiameterR = data.DiameterR;
        Axisr = data.Axisr;
        Pdr = data.Pdr;
        Phr = data.Phr;
        BaseR = data.BaseR;
        CenterThicknessR = data.CenterThicknessR;
        KnifeSideR = data.KnifeSideR;
        DecenterR = data.DecenterR;
        PrismR = data.PrismR;
        QtyR = data.QtyR;

        SalesSpuIdL = data.SalesSpuIdL;
        SalesSpuCodeL = data.SalesSpuCodeL;
        CustomerSpuIdL = data.CustomerSpuIdL;
        CustomerSpuCodeL = data.CustomerSpuCodeL;
        SpuIdL = data.SpuIdL;
        WarehouseIdL = data.WarehouseIdL;
        DegreesTypeL = data.DegreesTypeL;
        Sphl = data.Sphl;
        Cyll = data.Cyll;
        Addl = data.Addl;
        DiameterL = data.DiameterL;
        Axisl = data.Axisl;
        Pdl = data.Pdl;
        Phl = data.Phl;
        BaseL = data.BaseL;
        CenterThicknessL = data.CenterThicknessL;
        KnifeSideL = data.KnifeSideL;
        DecenterL = data.DecenterL;
        PrismL = data.PrismL;
        QtyL = data.QtyL;
        Remark = data.Remark;

        AddDomainEvent(new TripleInvoiceAddedEvent(this));
    }

    /// <summary>
    /// 序列号类型
    /// </summary>
    /// <returns>序列号类型</returns>
    public override string GetSerialNumberType()
    {
        return SerialNumberTypeEnum.TripleInvoice;
    }

    /// <summary>
    /// 分配单号
    /// </summary>
    /// <param name="number">单号前缀</param>
    public override void AssignNumber(string number)
    {
        base.AssignNumber(number + StoreCode);
    }
}
