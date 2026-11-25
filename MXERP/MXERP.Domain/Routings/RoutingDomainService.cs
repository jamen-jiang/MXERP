using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Routings;

[ScopedService]
public class RoutingDomainService : IRoutingDomainService
{
    private readonly IRoutingFactory _factory;
    private readonly IRoutingRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">路线工厂</param>
    /// <param name="repository">路线仓储</param>
    public RoutingDomainService(IRoutingFactory factory, IRoutingRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Routing> CreateAsync(string code, string name, string? remark, List<RoutingStepData>? steps, CancellationToken cancellationToken = default)
    {
        Routing model = await _factory.CreateAsync(code, name, remark, cancellationToken);
        await SyncRoutingStepsAsync(model, steps, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task<Routing> ModifyAsync(long id, string name, string? remark, List<RoutingStepData>? steps, CancellationToken cancellationToken = default)
    {
        Routing? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException($"路线{id}不存在!");
        }

        model.Modify(name, remark);
        await SyncRoutingStepsAsync(model, steps, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public Task SyncRoutingStepsAsync(Routing routing, List<RoutingStepData>? steps, CancellationToken cancellationToken = default)
    {
        List<RoutingStepData> inputs = steps ?? new List<RoutingStepData>();

        List<RoutingStep> toRemoves = routing.RoutingSteps
            .Where(s => inputs.All(d => d.WorkStepId != s.WorkStepId))
            .ToList();
        foreach (RoutingStep step in toRemoves)
        {
            routing.DeleteStep(step.Id);
        }

        Dictionary<long, long> existingDict = new Dictionary<long, long>();
        foreach (RoutingStep step in routing.RoutingSteps)
        {
            if (!step.IsDeleted && !existingDict.ContainsKey(step.WorkStepId))
            {
                existingDict[step.WorkStepId] = step.Id;
            }
        }

        foreach (RoutingStepData data in inputs)
        {
            if (existingDict.TryGetValue(data.WorkStepId, out long id))
            {
                routing.ModifyStep(id, data.WorkStepId, data.Sequence, data.IsMust, data.TimeInMins, data.Remark);
            }
            else
            {
                routing.AddStep(data.WorkStepId, data.Sequence, data.IsMust, data.TimeInMins, data.Remark);
            }
        }

        return Task.CompletedTask;
    }
}
