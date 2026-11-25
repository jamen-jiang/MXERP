namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 销售产品分配项数据
/// </summary>
/// <param name="Sequence">序列</param>
/// <param name="SpuId">产品Id</param>
/// <param name="WarehouseId">仓库Id</param>
/// <param name="IsPriorityCustomized">是否优先订购</param>
public sealed record SalesSpuAssignItemData(int? Sequence, long? SpuId, long? WarehouseId, bool? IsPriorityCustomized);
