using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.SpuBrands;

/// <summary>
/// 产品品牌
/// </summary>
public class SpuBrand : AuditedAggregateRoot
{
    /// <summary>
    /// 品牌代码
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
    /// 构造器(供 EF 使用)
    /// </summary>
    protected SpuBrand()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="code">品牌代码</param>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注(可空)</param>
    internal SpuBrand(string code, string name, string? remark)
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
    /// 修改品牌
    /// </summary>
    /// <param name="name">品牌名称</param>
    /// <param name="remark">备注(可空)</param>
    public void Modify(string name, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("品牌名称不能为空");
        }

        Name = name.Trim();
        Remark = remark;
    }
}
