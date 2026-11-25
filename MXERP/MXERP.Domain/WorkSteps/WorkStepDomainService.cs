using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.WorkSteps;

/// <summary>
/// 工作步骤领域服务(对应 Java WorkStepDomainService): 负责序列编排
/// </summary>
[ScopedService]
public class WorkStepDomainService : IWorkStepDomainService
{
    private readonly IWorkStepRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public WorkStepDomainService(IWorkStepRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<WorkStepGenerateResult> CreateAsync(
        long departmentId,
        string code,
        string name,
        bool isMust,
        string? remark,
        int prevSequence,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException("工作步骤代码已存在!");
        }

        if (await _repository.IsExistByNameExcludeIdAsync(name, null, cancellationToken))
        {
            throw new UserFriendlyException("工作步骤名称已存在!");
        }

        if (prevSequence != 0 && await _repository.GetBySequenceAsync(prevSequence, cancellationToken) is null)
        {
            throw new UserFriendlyException($"上一个序列{prevSequence}的工作步骤不存在!");
        }

        var model = new WorkStep(departmentId, code, name, isMust, remark);
        model.ChangeSequence(prevSequence == 0 ? 1 : prevSequence + 1);
        List<WorkStep> toResequence = await DoResequenceAsync(model.Sequence, null, cancellationToken);
        return new WorkStepGenerateResult(model, toResequence);
    }

    /// <inheritdoc />
    public async Task<WorkStepGenerateResult> ModifyAsync(
        long id,
        long departmentId,
        string code,
        string name,
        bool isMust,
        string? remark,
        int prevSequence,
        CancellationToken cancellationToken = default)
    {
        if (prevSequence != 0 && await _repository.GetBySequenceAsync(prevSequence, cancellationToken) is null)
        {
            throw new UserFriendlyException($"上一个序列{prevSequence}的工作步骤不存在!");
        }

        WorkStep? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("工作步骤不存在!");
        }

        await ModifyAsync(model, departmentId, code, name, isMust, remark, cancellationToken);
        model.ChangeSequence(prevSequence == 0 ? 1 : prevSequence + 1);
        List<WorkStep> toResequence = await DoResequenceAsync(model.Sequence, id, cancellationToken);
        return new WorkStepGenerateResult(model, toResequence);
    }

    /// <inheritdoc />
    public async Task ModifyAsync(
        WorkStep model,
        long departmentId,
        string code,
        string name,
        bool isMust,
        string? remark,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("工作步骤代码已存在!");
        }

        if (await _repository.IsExistByNameExcludeIdAsync(name, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("工作步骤名称已存在!");
        }

        model.Modify(departmentId, name, isMust, remark);
    }

    /// <inheritdoc />
    public async Task<List<WorkStep>> RemoveAndResequenceAsync(long id, CancellationToken cancellationToken = default)
    {
        var toSave = new List<WorkStep>();
        WorkStep? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is not null)
        {
            model.Delete();
            toSave.Add(model);
        }

        List<WorkStep> surplus = await _repository.GetListExcludeIdsAsync(new List<long> { id }, cancellationToken);
        int sequence = 1;
        foreach (WorkStep item in surplus.OrderBy(x => x.Sequence))
        {
            item.ChangeSequence(sequence);
            sequence++;
        }

        toSave.AddRange(surplus);
        return toSave;
    }

    /// <summary>
    /// DoResequenceAsync(工序)
    /// </summary>
    /// <param name="targetSequence">targetSequence</param>
    /// <param name="excludeId">excludeId</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工序列表</returns>
    private async Task<List<WorkStep>> DoResequenceAsync(int targetSequence, long? excludeId, CancellationToken cancellationToken)
    {
        List<WorkStep> others = excludeId is null
            ? await _repository.GetAllOrderBySequenceAsync(cancellationToken)
            : await _repository.GetListExcludeIdAsync(excludeId.Value, cancellationToken);

        var result = new List<WorkStep>();
        int sequence = 1;
        foreach (WorkStep other in others.OrderBy(x => x.Sequence))
        {
            if (sequence == targetSequence)
            {
                sequence++;
            }

            other.ChangeSequence(sequence);
            result.Add(other);
            sequence++;
        }

        return result;
    }
}
