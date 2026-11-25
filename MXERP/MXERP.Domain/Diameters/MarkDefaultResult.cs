namespace MXERP.Domain.Diameters;

/// <summary>
/// 标记默认结果(对应 Java DiameterDomainService.MarkDefaultResult)
/// </summary>
public sealed record MarkDefaultResult(Diameter? OldDefault, Diameter NewDefault);
