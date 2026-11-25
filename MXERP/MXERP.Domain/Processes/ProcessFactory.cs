using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Processes;

[ScopedService]
public class ProcessFactory : IProcessFactory
{
    private readonly IProcessRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">工序仓储</param>
    public ProcessFactory(IProcessRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Process> CreateAsync(ProcessTypeEnum type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByTypeAndCodeExcludeIdAsync(type, code, null, cancellationToken))
        {
            throw new UserFriendlyException($"工序代码{code}已存在!");
        }

        return new Process(type, code, name, price, cost, isCFUsable, isKCUsable, remark);
    }
}
