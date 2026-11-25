using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Departments;

/// <summary>
/// 部门领域服务(对应 Java DepartmentDomainService): 负责层级编排
/// </summary>
[ScopedService]
public class DepartmentDomainService : IDepartmentDomainService
{
    private readonly IDepartmentRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public DepartmentDomainService(IDepartmentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<DepartmentGenerateResult> CreateAsync(
        long companyId,
        long? parentId,
        string code,
        string name,
        string? numberCode,
        string? telephone,
        string? fax,
        string? email,
        int? sort,
        string? address,
        string? remark,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeAndCompanyIdExcludeIdAsync(code, companyId, null, cancellationToken))
        {
            throw new UserFriendlyException($"部门代码{code}已存在!");
        }

        string? level = await GetLevelAsync(parentId, cancellationToken);
        var department = new Department(level, companyId, parentId, code, name, numberCode, telephone, fax, email, sort, address, remark);
        var departments = new List<Department>();
        await SetChildrenLevelAsync(department, departments, cancellationToken);
        return new DepartmentGenerateResult(department, departments);
    }

    /// <inheritdoc />
    public async Task<DepartmentGenerateResult> ModifyAsync(
        long id,
        long companyId,
        long? parentId,
        string code,
        string name,
        string? numberCode,
        string? telephone,
        string? fax,
        string? email,
        int? sort,
        string? address,
        string? remark,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeAndCompanyIdExcludeIdAsync(code, companyId, id, cancellationToken))
        {
            throw new UserFriendlyException($"部门代码{code}已存在!");
        }

        Department? department = await _repository.FindAsync(id, true, cancellationToken);
        if (department is null)
        {
            throw new UserFriendlyException("部门不存在!");
        }

        string? level = await GetLevelAsync(parentId, cancellationToken);
        department.Modify(level, companyId, parentId, code, name, numberCode, telephone, fax, email, sort, address, remark);
        var departments = new List<Department>();
        await SetChildrenLevelAsync(department, departments, cancellationToken);
        return new DepartmentGenerateResult(department, departments);
    }

    /// <inheritdoc />
    public async Task<List<Department>> RemoveWithChildrenAsync(long id, CancellationToken cancellationToken = default)
    {
        List<Department> list = await _repository.GetWithChildrenAsync(id, cancellationToken);
        foreach (Department item in list)
        {
            item.Delete();
        }

        return list;
    }

    /// <summary>
    /// GetLevelAsync(部门)
    /// </summary>
    /// <param name="parentId">上级Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部门, 不存在返回 null</returns>
    private async Task<string?> GetLevelAsync(long? parentId, CancellationToken cancellationToken)
    {
        if (parentId is null)
        {
            return null;
        }

        Department? parent = await _repository.FindAsync(parentId.Value, true, cancellationToken);
        if (parent is null)
        {
            throw new UserFriendlyException($"父部门不存在: {parentId}");
        }

        if (parent.Level.IsNullOrWhiteSpace())
        {
            return parent.Id.ToString();
        }

        return parent.Level + "," + parent.Id;
    }

    /// <summary>
    /// SetChildrenLevelAsync(部门)
    /// </summary>
    /// <param name="department">department</param>
    /// <param name="list">list</param>
    /// <param name="cancellationToken">取消令牌</param>
    private async Task SetChildrenLevelAsync(Department department, List<Department> list, CancellationToken cancellationToken)
    {
        if (department.Id == 0)
        {
            return;
        }

        List<Department> children = await _repository.GetListByParentIdAsync(department.Id, cancellationToken);
        if (children.Count == 0)
        {
            return;
        }

        foreach (Department child in children)
        {
            if (department.Level.IsNullOrWhiteSpace())
            {
                child.ChangeLevel(department.Id.ToString());
            }
            else
            {
                child.ChangeLevel(department.Level + "," + department.Id);
            }

            await SetChildrenLevelAsync(child, list, cancellationToken);
            list.Add(child);
        }
    }
}
