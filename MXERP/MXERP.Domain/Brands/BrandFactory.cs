using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Brands;

/// <summary>
/// 品牌工厂(对应 Java 的 BrandFactory): 负责创建前的代码唯一性校验
/// </summary>
[ScopedService]
public class BrandFactory : IBrandFactory
{
    private readonly IBrandRepository _repository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="repository">品牌仓储</param>
    public BrandFactory(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <inheritdoc />
    public async Task<Brand> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeExcludeIdAsync(code, null, cancellationToken))
        {
            throw new UserFriendlyException($"品牌代码{code}已存在!");
        }

        return new Brand(code, name, remark);
    }
}
