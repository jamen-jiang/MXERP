using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;
using Snow.DistributedLocking;
using Snow.Exceptions;

namespace MXERP.Infrastructure.SerialNumbers;

/// <summary>
/// 流水号生成器实现: 按流水号类型加分布式锁, 防止并发下取到同一个号。
/// <para>锁 key 与 Java 侧 <c>@DistributedLock(prefix = EntityConst.SERIALNUMBER, value = "#type")</c> 一致,
/// 即 <c>SerialNumber:{type}</c>; 单机部署用默认的进程内锁, 多实例由 <c>Snow.DistributedLocking.Redis</c> 顶替。</para>
/// <para>取号后立即落库(autoSave), 保证锁释放前新号已持久化 —— 否则并发请求会读到同一个旧号。</para>
/// </summary>
[ScopedService]
public class SerialNumberGenerator : ISerialNumberGenerator
{
    /// <summary>
    /// 锁 key 前缀
    /// </summary>
    private const string LockPrefix = "SerialNumber";

    private readonly ISerialNumberRepository _repository;
    private readonly ISnowDistributedLock _distributedLock;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">流水号仓储</param>
    /// <param name="distributedLock">分布式锁</param>
    public SerialNumberGenerator(ISerialNumberRepository repository, ISnowDistributedLock distributedLock)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _distributedLock = distributedLock ?? throw new ArgumentNullException(nameof(distributedLock));
    }

    /// <inheritdoc />
    public async Task<string> NextAsync(string type, CancellationToken cancellationToken = default)
    {
        await using ISnowDistributedLockHandle handle = await _distributedLock.TryAcquireAsync($"{LockPrefix}:{type}", cancellationToken: cancellationToken)
            ?? throw new UserFriendlyException($"流水号类型 {type} 加锁失败!");

        SerialNumber? serialNumber = await _repository.GetByTypeAsync(type, cancellationToken);
        if (serialNumber is null)
        {
            throw new UserFriendlyException($"流水号类型 {type} 未配置!");
        }

        string number = serialNumber.GetNextNumber();
        await _repository.UpdateAsync(serialNumber, true, cancellationToken);
        return number;
    }
}
