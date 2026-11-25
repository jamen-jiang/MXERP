using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Positions;

/// <summary>
/// 仓位(对应 Java Position)
/// </summary>
public class Position : AuditedAggregateRoot
{
    /// <summary>
    /// 分类
    /// </summary>
    public string? Category { get; private set; }
    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 简称
    /// </summary>
    public string? ShortName { get; private set; }
    /// <summary>
    /// 成品代码
    /// </summary>
    public string? EndProductCode { get; private set; }
    /// <summary>
    /// 半成品代码
    /// </summary>
    public string? SemiProductCode { get; private set; }
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
    private Position()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称</param>
    /// <param name="endProductCode">成品代码</param>
    /// <param name="semiProductCode">半成品代码</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal Position(string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓位代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓位名称不能为空");
        }

        Category = category;
        Code = code.Trim();
        Name = name.Trim();
        ShortName = shortName;
        EndProductCode = endProductCode;
        SemiProductCode = semiProductCode;
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改职位
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称</param>
    /// <param name="endProductCode">成品代码</param>
    /// <param name="semiProductCode">半成品代码</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal void Modify(string? category, string code, string name, string? shortName, string? endProductCode, string? semiProductCode, int? sort, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓位代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("仓位名称不能为空");
        }

        Category = category;
        Code = code.Trim();
        Name = name.Trim();
        ShortName = shortName;
        EndProductCode = endProductCode;
        SemiProductCode = semiProductCode;
        Sort = sort;
        Remark = remark;
    }
}
