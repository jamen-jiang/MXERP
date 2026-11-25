using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Ranks;

/// <summary>
/// 职级(对应 Java Rank, 继承 UpdateAggregateRoot)
/// </summary>
public class Rank : AuditedAggregateRoot
{
    /// <summary>
    /// 层级路径
    /// </summary>
    public string? Level { get; private set; }
    /// <summary>
    /// 上级Id
    /// </summary>
    public long? ParentId { get; private set; }
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
    private Rank()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="level">层级路径</param>
    /// <param name="parentId">上级Id</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal Rank(string? level, long? parentId, string name, int? sort, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("职级名称不能为空");
        }

        Level = level?.Trim();
        ParentId = parentId;
        Name = name.Trim();
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 修改职级
    /// </summary>
    /// <param name="level">层级路径</param>
    /// <param name="parentId">上级Id</param>
    /// <param name="name">名称</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal void Modify(string? level, long? parentId, string name, int? sort, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("职级名称不能为空");
        }

        Level = level?.Trim();
        ParentId = parentId;
        Name = name.Trim();
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// ChangeLevel(职级)
    /// </summary>
    /// <param name="level">层级路径</param>
    public void ChangeLevel(string level)
    {
        Level = level;
    }
}
