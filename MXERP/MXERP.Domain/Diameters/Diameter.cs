using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Diameters;

/// <summary>
/// 直径(对应 Java Diameter)
/// </summary>
public class Diameter : AuditedAggregateRoot
{
    /// <summary>
    /// 尺寸
    /// </summary>
    public int Size { get; private set; }
    /// <summary>
    /// 颜色
    /// </summary>
    public string Color { get; private set; } = string.Empty;
    /// <summary>
    /// 是否默认
    /// </summary>
    public bool IsDefault { get; private set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private Diameter()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <param name="color">颜色</param>
    /// <param name="remark">备注</param>
    internal Diameter(int size, string color, string? remark)
    {
        if (color.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("颜色不能为空");
        }

        Size = size;
        Color = color.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 修改直径
    /// </summary>
    /// <param name="color">颜色</param>
    /// <param name="remark">备注</param>
    internal void Modify(string color, string? remark)
    {
        if (color.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("颜色不能为空");
        }

        Color = color.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 设为默认直径
    /// </summary>
    internal void MarkAsDefault()
    {
        IsDefault = true;
    }

    /// <summary>
    /// 取消默认直径
    /// </summary>
    internal void UnmarkDefault()
    {
        IsDefault = false;
    }

    /// <summary>
    /// 删除(默认直径不允许删除)
    /// </summary>
    public override void Delete()
    {
        if (IsDefault)
        {
            throw new UserFriendlyException($"直径{Size}已设置默认,不允许删除!");
        }

        base.Delete();
    }
}
