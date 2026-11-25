using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Posts;

[ScopedService]
public class PostDomainService : IPostDomainService
{
    private readonly IPostFactory _factory;
    private readonly IPostRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">岗位工厂</param>
    /// <param name="repository">仓储</param>
    public PostDomainService(IPostFactory factory, IPostRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Post> ModifyAsync(long id, string name, long departmentId, long rankId, string? remark, List<long> roleIds, CancellationToken cancellationToken = default)
    {
        Post? post = await _repository.FindAsync(id, true, cancellationToken);
        if (post is null)
        {
            throw new UserFriendlyException("岗位不存在!");
        }

        await ModifyAsync(post, name, departmentId, rankId, remark, roleIds, cancellationToken);
        return post;
    }

    /// <inheritdoc />
    public Task ModifyAsync(Post post, string name, long departmentId, long rankId, string? remark, List<long> roleIds, CancellationToken cancellationToken = default)
    {
        post.Modify(name, departmentId, rankId, remark, roleIds);
        return Task.CompletedTask;
    }
}
