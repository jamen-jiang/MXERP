using Snow.DependencyInjection;
using Snow.Domain.Exceptions;

namespace MXERP.Domain.Brands;

/// <summary>品牌工厂(对应 Java 的 BrandFactory): 负责创建前的代码唯一性校验</summary>
[ScopedService]
public class BrandFactory
{
    private readonly IBrandRepository _repository;

    /// <summary>构造器</summary>
    /// <param name="repository">品牌仓储</param>
    public BrandFactory(IBrandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>创建一个新品牌, 代码重复抛领域异常</summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>新品牌</returns>
    public async Task<Brand> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default)
    {
        if (await _repository.IsExistByCodeAsync(code, cancellationToken))
        {
            throw new DomainException($"品牌代码{code}已存在!");
        }

        return new Brand(code, name, remark);
    }
}
