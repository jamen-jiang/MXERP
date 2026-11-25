namespace MXERP.Domain.WorkSteps;

/// <summary>
/// 工作步骤生成结果(对应 Java WorkStepDomainService.GenerateResult)
/// </summary>
public sealed record WorkStepGenerateResult(WorkStep Main, List<WorkStep> ToResequence);
