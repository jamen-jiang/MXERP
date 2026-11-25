using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Brands;

/// <summary>
/// 品牌聚合根(对应 Java com.mxerp.domain.brand.Brand, 继承 Java 的 UpdateAggregateRoot)。
/// <para>创建走 BrandFactory, 修改走 Modify。</para>
/// </summary>
public class Brand : AuditedAggregateRoot
{
    /// <summary>
    /// 品牌代码(唯一, 未删除行内唯一)
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 品牌名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// EF Core 物化用
    /// </summary>
    private Brand()
    {
    }

    /// <summary>
    /// 创建品牌(仅供 BrandFactory 调用)
    /// </summary>
    internal Brand(string code, string name, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("品牌代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("品牌名称不能为空");
        }

        Code = code.Trim();
        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 修改品牌代码、名称与备注(对应 Java 的 modify)
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注</param>
    public void Modify(string code, string name, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("品牌代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("品牌名称不能为空");
        }

        Code = code.Trim();
        Name = name.Trim();
        Remark = remark;
    }
}
