using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.EndProductCodeConfigs;

[ScopedService]
public class EndProductCodeConfigFactory : IEndProductCodeConfigFactory
{
    private readonly IEndProductCodeConfigRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">成品代码配置仓储</param>
    public EndProductCodeConfigFactory(IEndProductCodeConfigRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<EndProductCodeConfig> CreateAsync(string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistBySpuIdAndDegreesTypeExcludeIdAsync(spuId, degreesType, null, cancellationToken))
        {
            throw new UserFriendlyException($"产品Id {spuId} {degreesType} 已存在!");
        }

        return new EndProductCodeConfig(category, spuId, degreesType, sort);
    }
}
