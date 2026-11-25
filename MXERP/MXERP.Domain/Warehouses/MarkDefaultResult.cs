namespace MXERP.Domain.Warehouses;

/// <summary>
/// 标记默认结果(对应 Java WarehouseDomainService.MarkDefaultResult)
/// </summary>
public sealed record MarkDefaultResult(Warehouse? OldDefault, Warehouse NewDefault);
