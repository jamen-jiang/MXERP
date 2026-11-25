namespace MXERP.Domain.SalesSpus;

/// <summary>
/// 导入销售产品分配项数据
/// </summary>
/// <param name="Sequence">序列</param>
/// <param name="SpuCode">产品代码</param>
/// <param name="WarehouseCode">仓库代码</param>
/// <param name="IsPriorityCustomized">是否优先订购</param>
public sealed record ImportSalesSpuAssignItemData(int? Sequence, string SpuCode, string WarehouseCode, bool? IsPriorityCustomized);
