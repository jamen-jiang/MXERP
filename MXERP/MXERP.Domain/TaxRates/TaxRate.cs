using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.TaxRates;

/// <summary>
/// 税率(对应 Java TaxRate)
/// </summary>
public class TaxRate : AuditedAggregateRoot
{
    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 税率
    /// </summary>
    public decimal Rate { get; private set; }
    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; private set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private TaxRate()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="rate">rate</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal TaxRate(string code, string name, decimal rate, int? sort, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("税率代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("税率名称不能为空");
        }

        Code = code.Trim();
        Name = name.Trim();
        Rate = rate;
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改税率
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="rate">rate</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    public void Modify(string name, decimal rate, int? sort, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("税率名称不能为空");
        }

        Name = name.Trim();
        Rate = rate;
        Sort = sort;
        Remark = remark;
    }
}
