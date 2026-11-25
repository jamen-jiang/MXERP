using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Diameters;

[ScopedService]
public class DiameterFactory : IDiameterFactory
{
    private readonly IDiameterRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">仓储</param>
    public DiameterFactory(IDiameterRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Diameter> CreateAsync(int size, string color, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistBySizeExcludeIdAsync(size, null, cancellationToken))
        {
            throw new UserFriendlyException($"直径{size}已存在!");
        }

        if (await _repository.IsExistByColorExcludeIdAsync(color, null, cancellationToken))
        {
            throw new UserFriendlyException($"颜色{color}已存在!");
        }

        return new Diameter(size, color, remark);
    }
}
