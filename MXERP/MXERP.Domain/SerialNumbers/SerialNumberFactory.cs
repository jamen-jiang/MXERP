using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.SerialNumbers;

[ScopedService]
public class SerialNumberFactory : ISerialNumberFactory
{
    private readonly ISerialNumberRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">流水号仓储</param>
    public SerialNumberFactory(ISerialNumberRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<SerialNumber> CreateAsync(SerialNumberTypeEnum type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, DateTypeEnum? rule, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByTypeExcludeIdAsync(type, null, cancellationToken))
        {
            throw new UserFriendlyException($"类型{type}已存在!");
        }

        if (await _repository.IsExistByPrefixExcludeIdAsync(prefix, null, cancellationToken))
        {
            throw new UserFriendlyException($"前缀{prefix}已存在!");
        }

        return new SerialNumber(type, prefix, dateFormat, pad, padSymbol, isSplit, splitSymbol, rule);
    }
}
