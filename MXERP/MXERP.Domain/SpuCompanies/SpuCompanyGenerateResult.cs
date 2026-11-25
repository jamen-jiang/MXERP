namespace MXERP.Domain.SpuCompanies;

/// <summary>
/// 产品公司关联变更结果
/// </summary>
/// <param name="AddedSpuCompanies">新增的产品公司关联</param>
/// <param name="RemovedSpuCompanies">删除的产品公司关联</param>
public sealed record SpuCompanyGenerateResult(List<SpuCompany> AddedSpuCompanies, List<SpuCompany> RemovedSpuCompanies);
