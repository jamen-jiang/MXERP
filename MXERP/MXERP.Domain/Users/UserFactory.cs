using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Users;

[ScopedService]
public class UserFactory : IUserFactory
{
    private readonly IUserRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public UserFactory(IUserRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<User> CreateAsync(string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByNameExcludeIdAsync(name, null, cancellationToken))
        {
            throw new UserFriendlyException($"用户名称{name}已存在!");
        }
        if (await _repository.IsExistByUserNameExcludeIdAsync(userName, null, cancellationToken))
        {
            throw new UserFriendlyException($"用户账号{userName}已存在!");
        }
        if (!string.IsNullOrWhiteSpace(email) && await _repository.IsExistByEmailExcludeIdAsync(email, null, cancellationToken))
        {
            throw new UserFriendlyException($"用户邮箱{email}已存在!");
        }

        return new User(name, userName, email, password, remark, postIds, roleIds);
    }
}
