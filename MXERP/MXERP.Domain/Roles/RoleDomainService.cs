using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Roles;

[ScopedService]
public class RoleDomainService : IRoleDomainService
{
    private readonly RoleFactory _factory;
    private readonly IRoleRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">工厂</param>
    /// <param name="repository">仓储</param>
    public RoleDomainService(RoleFactory factory, IRoleRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Role> ModifyAsync(long id, string name, string? remark, CancellationToken cancellationToken = default)
    {
        Role? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("角色不存在!");
        }

        await ModifyAsync(model, name, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Role role, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByNameExcludeIdAsync(name, role.Id, cancellationToken))
        {
            throw new UserFriendlyException($"角色{name}已存在!");
        }

        role.Modify(name, remark);
    }
}
