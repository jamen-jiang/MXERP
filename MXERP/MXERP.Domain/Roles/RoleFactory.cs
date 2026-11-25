using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Roles;

[ScopedService]
public class RoleFactory : IRoleFactory
{
    private readonly IRoleRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public RoleFactory(IRoleRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Role> CreateAsync(string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByNameExcludeIdAsync(name, null, cancellationToken))
        {
            throw new UserFriendlyException($"角色{name}已存在!");
        }

        return new Role(name, remark);
    }
}
