namespace MXERP.Domain.Departments;

/// <summary>
/// 部门生成结果(对应 Java DepartmentDomainService.GenerateResult)
/// </summary>
public sealed record DepartmentGenerateResult(Department Department, List<Department> Departments);
