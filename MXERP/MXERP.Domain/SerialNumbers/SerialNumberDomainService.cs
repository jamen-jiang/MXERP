using Snow.DependencyInjection;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.SerialNumbers;

[ScopedService]
public class SerialNumberDomainService : ISerialNumberDomainService
{
    private readonly ISerialNumberRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">流水号仓储</param>
    public SerialNumberDomainService(ISerialNumberRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<SerialNumber> ModifyAsync(long id, SerialNumberTypeEnum type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, DateTypeEnum? rule, CancellationToken cancellationToken = default)
    {
        SerialNumber? serialNumber = await _repository.FindAsync(id, true, cancellationToken);
        if (serialNumber is null)
        {
            throw new UserFriendlyException("流水号不存在!");
        }

        await ModifyAsync(serialNumber, type, prefix, dateFormat, pad, padSymbol, isSplit, splitSymbol, rule, cancellationToken);
        return serialNumber;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(SerialNumber serialNumber, SerialNumberTypeEnum type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, DateTypeEnum? rule, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByTypeExcludeIdAsync(type, serialNumber.Id, cancellationToken))
        {
            throw new UserFriendlyException($"类型{type}已存在!");
        }

        if (await _repository.IsExistByPrefixExcludeIdAsync(prefix, serialNumber.Id, cancellationToken))
        {
            throw new UserFriendlyException($"前缀{prefix}已存在!");
        }

        serialNumber.Modify(type, prefix, dateFormat, pad, padSymbol, isSplit, splitSymbol, rule);
    }
}
