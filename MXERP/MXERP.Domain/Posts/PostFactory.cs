using Snow.DependencyInjection;

namespace MXERP.Domain.Posts;

[ScopedService]
public class PostFactory : IPostFactory
{
    private readonly IPostRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public PostFactory(IPostRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public Task<Post> CreateAsync(string name, long departmentId, long rankId, string? remark, List<long> roleIds, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new Post(name, departmentId, rankId, remark, roleIds));
    }
}
