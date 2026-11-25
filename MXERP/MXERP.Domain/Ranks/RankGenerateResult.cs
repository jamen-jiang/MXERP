namespace MXERP.Domain.Ranks;

/// <summary>
/// 职级生成结果(对应 Java RankDomainService.GenerateResult)
/// </summary>
public sealed record RankGenerateResult(Rank Rank, List<Rank> Ranks);
