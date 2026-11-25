namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单数据(对应 Java RetailOrderData)
/// </summary>
/// <param name="Id">Id(可空)</param>
/// <param name="StoreId">门店Id</param>
/// <param name="CompanyId">公司Id</param>
/// <param name="OutArea">传出区(可空)</param>
/// <param name="SequenceNumber">流水号(可空)</param>
/// <param name="HkNumber">流水号HK(可空)</param>
/// <param name="CustomerOrderNumber">客户单号</param>
/// <param name="CustomerPONumber">客户PO号(可空)</param>
/// <param name="DocumentaryBarCode">跟单条码(可空)</param>
/// <param name="OutGood">拟出货</param>
/// <param name="OutGoodTime">拟出时段(可空)</param>
/// <param name="Material">玻璃胶片(可空)</param>
/// <param name="SalesSpuIdR">右眼: 销售产品Id</param>
/// <param name="CustomerSpuIdR">右眼: 客人产品Id(可空)</param>
/// <param name="SpuIdR">右眼: 产品Id</param>
/// <param name="WarehouseIdR">右眼: 仓库Id</param>
/// <param name="SphR">右眼: SPH</param>
/// <param name="CylR">右眼: CYL</param>
/// <param name="AddR">右眼: ADD(可空)</param>
/// <param name="DiameterR">右眼: 直径(可空)</param>
/// <param name="AxisR">右眼: 轴位(可空)</param>
/// <param name="PdR">右眼: 瞳距(可空)</param>
/// <param name="PhR">右眼: 镜高(可空)</param>
/// <param name="BaseR">右眼: 面弯(可空)</param>
/// <param name="CenterThicknessR">右眼: 中心厚度(可空)</param>
/// <param name="KnifeSideR">右眼: 刀边(可空)</param>
/// <param name="DecenterR">右眼: 移心(可空)</param>
/// <param name="PrismR">右眼: 棱镜(可空)</param>
/// <param name="QtyR">右眼: 数量</param>
/// <param name="SalesSpuIdL">左眼: 销售产品Id</param>
/// <param name="CustomerSpuIdL">左眼: 客人产品Id(可空)</param>
/// <param name="SpuIdL">左眼: 产品Id</param>
/// <param name="WarehouseIdL">左眼: 仓库Id</param>
/// <param name="SphL">左眼: SPH</param>
/// <param name="CylL">左眼: CYL</param>
/// <param name="AddL">左眼: ADD(可空)</param>
/// <param name="DiameterL">左眼: 直径(可空)</param>
/// <param name="AxisL">左眼: 轴位(可空)</param>
/// <param name="PdL">左眼: 瞳距(可空)</param>
/// <param name="PhL">左眼: 镜高(可空)</param>
/// <param name="BaseL">左眼: 面弯(可空)</param>
/// <param name="CenterThicknessL">左眼: 中心厚度(可空)</param>
/// <param name="KnifeSideL">左眼: 刀边(可空)</param>
/// <param name="DecenterL">左眼: 移心(可空)</param>
/// <param name="PrismL">左眼: 棱镜(可空)</param>
/// <param name="QtyL">左眼: 数量</param>
/// <param name="Coating">焗色(可空)</param>
/// <param name="TintingColor">茶色(可空)</param>
/// <param name="Mirror">水银(可空)</param>
/// <param name="TintingCode">染色代号(可空)</param>
/// <param name="TintingName">染色名称(可空)</param>
/// <param name="Uv">UV(可空)</param>
/// <param name="Hard">加硬(可空)</param>
/// <param name="Polish">抛光(可空)</param>
/// <param name="Auro">彩边(可空)</param>
/// <param name="Edge">裁型/车边(可空)</param>
/// <param name="SuperHole">开坑(可空)</param>
/// <param name="Special">批花(可空)</param>
/// <param name="SpecialCode">批花代号(可空)</param>
/// <param name="ZuanKong">钻孔(可空)</param>
/// <param name="Frame">镜架(可空)</param>
/// <param name="Remark">备注(可空)</param>
public sealed record RetailOrderData(long? Id, long StoreId, long CompanyId, string? OutArea, string? SequenceNumber, string? HkNumber, string CustomerOrderNumber, string? CustomerPONumber, string? DocumentaryBarCode, int OutGood, string? OutGoodTime, string? Material, long SalesSpuIdR, long? CustomerSpuIdR, long SpuIdR, long WarehouseIdR, int SphR, int CylR, int? AddR, int? DiameterR, int? AxisR, double? PdR, double? PhR, string? BaseR, double? CenterThicknessR, bool? KnifeSideR, string? DecenterR, string? PrismR, double QtyR, long SalesSpuIdL, long? CustomerSpuIdL, long SpuIdL, long WarehouseIdL, int SphL, int CylL, int? AddL, int? DiameterL, int? AxisL, double? PdL, double? PhL, string? BaseL, double? CenterThicknessL, bool? KnifeSideL, string? DecenterL, string? PrismL, double QtyL, string? Coating, string? TintingColor, string? Mirror, string? TintingCode, string? TintingName, bool? Uv, bool? Hard, bool? Polish, string? Auro, string? Edge, string? SuperHole, string? Special, string? SpecialCode, string? ZuanKong, string? Frame, string? Remark);
