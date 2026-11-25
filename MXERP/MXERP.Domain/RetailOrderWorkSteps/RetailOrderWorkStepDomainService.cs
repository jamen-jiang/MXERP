using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.RetailOrderWorkSteps;

/// <summary>
/// 散镜订单工作步骤领域服务，负责跨聚合的序列编排
/// </summary>
[ScopedService]
public class RetailOrderWorkStepDomainService : IRetailOrderWorkStepDomainService
{
    private readonly IRetailOrderWorkStepRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public RetailOrderWorkStepDomainService(IRetailOrderWorkStepRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<RetailOrderWorkStepGenerateResult> CreateAsync(long departmentId, string code, string name, string? remark, int prevSequence, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException("散镜订单工作步骤代码已存在!");
        }

        if (await _repository.IsExistByNameExcludeIdAsync(name, null, cancellationToken))
        {
            throw new UserFriendlyException("散镜订单工作步骤名称已存在!");
        }

        if (prevSequence != 0 && await _repository.GetBySequenceAsync(prevSequence, cancellationToken) is null)
        {
            throw new UserFriendlyException($"上一个序列{prevSequence}的散镜订单工作步骤不存在!");
        }

        RetailOrderWorkStep model = new RetailOrderWorkStep(departmentId, code, name, remark);
        if (prevSequence == 0)
        {
            model.ChangeSequence(1);
        }
        else
        {
            model.ChangeSequence(prevSequence + 1);
        }

        List<RetailOrderWorkStep> toResequence = await DoResequenceAsync(model.Sequence, null, cancellationToken);
        return new RetailOrderWorkStepGenerateResult(model, toResequence);
    }

    /// <inheritdoc />
    public async Task<RetailOrderWorkStepGenerateResult> ModifyAsync(long id, long departmentId, string code, string name, string? remark, int prevSequence, CancellationToken cancellationToken = default)
    {
        if (prevSequence != 0 && await _repository.GetBySequenceAsync(prevSequence, cancellationToken) is null)
        {
            throw new UserFriendlyException($"上一个序列{prevSequence}的散镜订单工作步骤不存在!");
        }

        RetailOrderWorkStep? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("散镜订单工作步骤不存在!");
        }

        await ModifyAsync(model, departmentId, code, name, remark, cancellationToken);

        if (prevSequence == 0)
        {
            model.ChangeSequence(1);
        }
        else
        {
            model.ChangeSequence(prevSequence + 1);
        }

        List<RetailOrderWorkStep> toResequence = await DoResequenceAsync(model.Sequence, id, cancellationToken);
        return new RetailOrderWorkStepGenerateResult(model, toResequence);
    }

    /// <inheritdoc />
    public async Task ModifyAsync(RetailOrderWorkStep model, long departmentId, string code, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("散镜订单工作步骤代码已存在!");
        }

        if (await _repository.IsExistByNameExcludeIdAsync(name, model.Id, cancellationToken))
        {
            throw new UserFriendlyException("散镜订单工作步骤名称已存在!");
        }

        model.Modify(departmentId, name, remark);
    }

    /// <inheritdoc />
    public async Task<List<RetailOrderWorkStep>> RemoveAndResequenceAsync(long? id, CancellationToken cancellationToken = default)
    {
        List<RetailOrderWorkStep> toSave = [];
        if (id is null)
        {
            return toSave;
        }

        RetailOrderWorkStep? model = await _repository.FindAsync(id.Value, true, cancellationToken);
        if (model is not null)
        {
            model.Delete();
            toSave.Add(model);
        }

        List<RetailOrderWorkStep> surplus = await _repository.GetListExcludeIdsAsync([id.Value], cancellationToken);
        int sequence = 1;
        foreach (RetailOrderWorkStep item in surplus.OrderBy(x => x.Sequence))
        {
            item.ChangeSequence(sequence);
            sequence++;
        }

        toSave.AddRange(surplus);
        return toSave;
    }

    /// <summary>
    /// 执行序列重排（跳过目标序列）
    /// </summary>
    /// <param name="targetSequence">目标序列</param>
    /// <param name="excludeId">排除的Id（可空，新建时传 null）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>需重排的步骤列表</returns>
    private async Task<List<RetailOrderWorkStep>> DoResequenceAsync(int targetSequence, long? excludeId, CancellationToken cancellationToken)
    {
        List<RetailOrderWorkStep> others;
        if (excludeId is null)
        {
            others = await _repository.GetAllOrderBySequenceAsync(cancellationToken);
        }
        else
        {
            others = await _repository.GetListExcludeIdAsync(excludeId.Value, cancellationToken);
        }

        List<RetailOrderWorkStep> result = new(others.Count);
        int sequence = 1;
        foreach (RetailOrderWorkStep other in others.OrderBy(x => x.Sequence))
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
