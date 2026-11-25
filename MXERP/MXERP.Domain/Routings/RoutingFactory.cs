using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Routings;

[ScopedService]
public class RoutingFactory : IRoutingFactory
{
    private readonly IRoutingRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">路线仓储</param>
    public RoutingFactory(IRoutingRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Routing> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"路线代码{code}已存在!");
        }

        return new Routing(code, name, remark);
    }
}
