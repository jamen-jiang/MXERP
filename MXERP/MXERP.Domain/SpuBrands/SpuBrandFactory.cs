using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.SpuBrands;

[ScopedService]
public class SpuBrandFactory : ISpuBrandFactory
{
    private readonly ISpuBrandRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">品牌仓储</param>
    public SpuBrandFactory(ISpuBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<SpuBrand> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"品牌代码{code}已存在!");
        }

        return new SpuBrand(code, name, remark);
    }
}
