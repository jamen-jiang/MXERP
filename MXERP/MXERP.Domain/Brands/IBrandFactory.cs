namespace MXERP.Domain.Brands;

/// <summary>
/// 品牌工厂接口(对应 Java 的 IBrandFactory): 负责创建前的代码唯一性校验。
/// </summary>
public interface IBrandFactory
{
    /// <summary>
    /// 创建一个新品牌, 代码重复抛业务异常
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>新品牌</returns>
    Task<Brand> CreateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default);
}
