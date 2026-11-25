namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品路线数据
/// </summary>
/// <param name="SpuId">产品Id</param>
/// <param name="RoutingId">路线Id</param>
public sealed record SalesSpuRoutingData(long? SpuId, long? RoutingId);
