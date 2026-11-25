using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Units;

/// <summary>
/// 单位(对应 Java Unit)
/// </summary>
public class Unit : AuditedAggregateRoot
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
    private Unit()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal Unit(string code, string name, int? sort, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("单位代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("单位名称不能为空");
        }

        Code = code.Trim();
        Name = name.Trim();
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改单位
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    public void Modify(string name, int? sort, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("单位名称不能为空");
        }

        Name = name.Trim();
        Sort = sort;
        Remark = remark;
    }
}
