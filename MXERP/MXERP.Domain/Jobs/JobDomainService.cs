using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Jobs;

[ScopedService]
public class JobDomainService : IJobDomainService
{
    private readonly IJobRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public JobDomainService(IJobRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Job> ModifyAsync(long id, string name, string beanName, string methodName, string? cron, string? remark, CancellationToken cancellationToken = default)
    {
        Job? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("任务不存在!");
        }

        await ModifyAsync(model, name, beanName, methodName, cron, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Job job, string name, string beanName, string methodName, string? cron, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByNameExcludeIdAsync(name, job.Id, cancellationToken))
        {
            throw new UserFriendlyException($"任务名称{name}已存在!");
        }

        if (await _repository.IsExistByBeanAndMethodExcludeIdAsync(beanName, methodName, job.Id, cancellationToken))
        {
            throw new UserFriendlyException($"Bean名称{beanName}和方法名称{methodName}的组合已存在!");
        }

        job.Modify(name, beanName, methodName, cron, remark);
    }
}
