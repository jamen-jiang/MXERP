namespace MXERP.Domain.Brands;

/// <summary>
/// 品牌领域服务接口(对应 Java 的 IBrandDomainService): 导入式生成 —— 查不到就新建, 查到就修改。
/// </summary>
public interface IBrandDomainService
{
    /// <summary>
    /// 按代码导入生成品牌
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>新建或已更新的品牌</returns>
    Task<Brand> ImportGenerateAsync(string code, string name, string? remark, CancellationToken cancellationToken = default);
}
