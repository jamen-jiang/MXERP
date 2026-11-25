using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Processes;

[ScopedService]
public class ProcessDomainService : IProcessDomainService
{
    private readonly ProcessFactory _factory;
    private readonly IProcessRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">工序工厂</param>
    /// <param name="repository">工序仓储</param>
    public ProcessDomainService(ProcessFactory factory, IProcessRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Process> ModifyAsync(long id, string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default)
    {
        Process? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("工序不存在!");
        }

        await ModifyAsync(model, type, code, name, price, cost, isCFUsable, isKCUsable, remark, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Process process, string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByTypeAndCodeExcludeIdAsync(type, code, process.Id, cancellationToken))
        {
            throw new UserFriendlyException($"工序代码{code}已存在!");
        }

        process.Modify(type, code, name, price, cost, isCFUsable, isKCUsable, remark);
    }

    /// <inheritdoc />
    public async Task<Process> ImportGenerateAsync(string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default)
    {
        Process? model = await _repository.GetByTypeAndCodeAsync(type, code, cancellationToken);
        if (model is null)
        {
            model = await _factory.CreateAsync(type, code, name, price, cost, isCFUsable, isKCUsable, remark, cancellationToken);
        }
        else
        {
            await ModifyAsync(model, type, code, name, price, cost, isCFUsable, isKCUsable, remark, cancellationToken);
        }

        return model;
    }
}
