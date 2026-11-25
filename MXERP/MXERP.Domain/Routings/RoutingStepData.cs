namespace MXERP.Domain.Routings;

/// <summary>
/// 路线工序行数据
/// </summary>
/// <param name="WorkStepId">工序Id</param>
/// <param name="Sequence">路线内顺序(相同值 = 并行工序)</param>
/// <param name="IsMust">是否必须</param>
/// <param name="TimeInMins">工时定额(可空)</param>
/// <param name="Remark">备注(可空)</param>
public sealed record RoutingStepData(long WorkStepId, int Sequence, bool IsMust, int? TimeInMins, string? Remark);
