using MXERP.Domain.Common.Enums;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.EndProductCodeConfigs;

[ScopedService]
public class EndProductCodeConfigDomainService : IEndProductCodeConfigDomainService
{
    private readonly EndProductCodeConfigFactory _factory;
    private readonly IEndProductCodeConfigRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="factory">成品代码配置工厂</param>
    /// <param name="repository">成品代码配置仓储</param>
    public EndProductCodeConfigDomainService(EndProductCodeConfigFactory factory, IEndProductCodeConfigRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<EndProductCodeConfig> ModifyAsync(long id, string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default)
    {
        EndProductCodeConfig? model = await _repository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("成品代码配置不存在!");
        }

        await ModifyAsync(model, category, spuId, degreesType, sort, cancellationToken);
        return model;
    }

    /// <inheritdoc />
    public async Task ModifyAsync(EndProductCodeConfig model, string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistBySpuIdAndDegreesTypeExcludeIdAsync(spuId, degreesType, model.Id, cancellationToken))
        {
            throw new UserFriendlyException($"产品Id {spuId} {degreesType} 已存在!");
        }

        model.Modify(category, spuId, degreesType, sort);
    }

    /// <inheritdoc />
    public async Task<EndProductCodeConfig> ImportGenerateAsync(string category, long spuId, string degreesType, int? sort, CancellationToken cancellationToken = default)
    {
        EndProductCodeConfig? model = await _repository.GetBySpuIdAndDegreesTypeAsync(spuId, degreesType, cancellationToken);
        if (model is null)
        {
            model = await _factory.CreateAsync(category, spuId, degreesType, sort, cancellationToken);
        }
        else
        {
            await ModifyAsync(model, category, spuId, degreesType, sort, cancellationToken);
        }

        return model;
    }
}
