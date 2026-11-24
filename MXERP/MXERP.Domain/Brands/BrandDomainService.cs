using Snow.DependencyInjection;

namespace MXERP.Domain.Brands;

/// <summary>
/// 品牌领域服务(对应 Java 的 BrandDomainService): 导入式生成 —— 查不到就新建, 查到就修改。
/// <para>不负责保存, 由调用方(应用服务/控制器)统一提交, 便于组合多个聚合根的同一事务。</para>
/// </summary>
[ScopedService]
public class BrandDomainService
{
    private readonly BrandFactory _factory;
    private readonly IBrandRepository _repository;

    /// <summary>构造器</summary>
    /// <param name="factory">品牌工厂</param>
    /// <param name="repository">品牌仓储</param>
    public BrandDomainService(BrandFactory factory, IBrandRepository repository)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    /// <summary>按代码导入生成品牌</summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>新建或已更新的品牌</returns>
    public async Task<Brand> ImportGenerateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default)
    {
        Brand? brand = await _repository.GetByCodeAsync(code, cancellationToken);
        if (brand is null)
        {
            return await _factory.CreateAsync(code, name, remark, cancellationToken);
        }

        brand.Modify(code, name, remark);
        return brand;
    }
}
