namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票数据(用于创建三联发票)
/// </summary>
public sealed record TripleInvoiceData(string CustomerInvoiceTitleNumber, long RetailOrderId, string RetailOrderNumber, string RetailOrderCustomerOrderNumber, string? RetailOrderCustomerPONumber, long StoreId, long CompanyId, string StoreCode, string StoreNumber, string StoreInvoiceTitle, string? StoreAddress, string? StoreTelephone, long SalesSpuIdR, string SalesSpuCodeR, long? CustomerSpuIdR, string? CustomerSpuCodeR, long SpuIdR, long WarehouseIdR, string DegreesTypeR, int Sphr, int Cylr, int? Addr, int? DiameterR, int? Axisr, double? Pdr, double? Phr, string? BaseR, double? CenterThicknessR, bool? KnifeSideR, string? DecenterR, string? PrismR, double QtyR, long SalesSpuIdL, string SalesSpuCodeL, long? CustomerSpuIdL, string? CustomerSpuCodeL, long SpuIdL, long WarehouseIdL, string DegreesTypeL, int Sphl, int Cyll, int? Addl, int? DiameterL, int? Axisl, double? Pdl, double? Phl, string? BaseL, double? CenterThicknessL, bool? KnifeSideL, string? DecenterL, string? PrismL, double QtyL, string? Remark);
