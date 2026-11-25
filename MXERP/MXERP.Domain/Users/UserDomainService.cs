using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Users;

[ScopedService]
public class UserDomainService : IUserDomainService
{
    private readonly IUserRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">用户工厂</param>
    /// <param name="repository">仓储</param>
    public UserDomainService(IUserFactory factory, IUserRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<User> ModifyAsync(long id, string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds, CancellationToken cancellationToken = default)
    {
        User? user = await _repository.FindAsync(id, true, cancellationToken);
        if (user is null)
        {
            throw new UserFriendlyException("用户不存在!");
        }

        await ModifyAsync(user, name, userName, email, password, remark, postIds, roleIds, cancellationToken);
        return user;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(User user, string name, string userName, string? email, string password, string? remark, List<long> postIds, List<long>? roleIds, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByNameExcludeIdAsync(name, user.Id, cancellationToken))
        {
            throw new UserFriendlyException($"用户名称{name}已存在!");
        }
        if (await _repository.IsExistByUserNameExcludeIdAsync(userName, user.Id, cancellationToken))
        {
            throw new UserFriendlyException($"用户账号{userName}已存在!");
        }
        if (!string.IsNullOrWhiteSpace(email) && await _repository.IsExistByEmailExcludeIdAsync(email, user.Id, cancellationToken))
        {
            throw new UserFriendlyException($"用户邮箱{email}已存在!");
        }

        user.Modify(name, userName, email, password, remark, postIds, roleIds);
    }
}
