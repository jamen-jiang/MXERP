namespace MXERP.Domain.RetailOrderWorkSteps;

/// <summary>
/// 散镜订单工作步骤生成结果
/// </summary>
/// <param name="Main">主步骤</param>
/// <param name="ToResequence">需重排的步骤列表</param>
public sealed record RetailOrderWorkStepGenerateResult(RetailOrderWorkStep Main, List<RetailOrderWorkStep> ToResequence);
