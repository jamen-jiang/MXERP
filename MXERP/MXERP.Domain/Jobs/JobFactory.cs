using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Jobs;

[ScopedService]
public class JobFactory : IJobFactory
{
    private readonly IJobRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public JobFactory(IJobRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Job> CreateAsync(string name, string beanName, string methodName, string? cron, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByNameExcludeIdAsync(name, null, cancellationToken))
        {
            throw new UserFriendlyException($"任务名称{name}已存在!");
        }

        if (await _repository.IsExistByBeanAndMethodExcludeIdAsync(beanName, methodName, null, cancellationToken))
        {
            throw new UserFriendlyException($"Bean名称{beanName}和方法名称{methodName}的组合已存在!");
        }

        return new Job(name, beanName, methodName, cron, remark);
    }
}
