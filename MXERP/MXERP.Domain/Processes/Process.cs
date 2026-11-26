using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Processes;

/// <summary>
/// 工序
/// </summary>
public class Process : AuditedAggregateRoot
{
    /// <summary>
    /// 工序类型
    /// </summary>
    public string Type { get; private set; } = string.Empty;

    /// <summary>
    /// 工序代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 工序名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// 成本
    /// </summary>
    public decimal Cost { get; private set; }

    /// <summary>
    /// 车房可用
    /// </summary>
    public bool IsCFUsable { get; private set; }

    /// <summary>
    /// 库存可用
    /// </summary>
    public bool IsKCUsable { get; private set; }

    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Process()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="name">工序名称</param>
    /// <param name="price">价格</param>
    /// <param name="cost">成本</param>
    /// <param name="isCFUsable">车房可用</param>
    /// <param name="isKCUsable">库存可用</param>
    /// <param name="remark">备注(可空)</param>
    internal Process(string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark)
    {
        string? trimmedCode = code?.Trim();
        string? trimmedName = name?.Trim();
        if (trimmedCode is null)
        {
            throw new UserFriendlyException("工序代码不能为空");
        }

        if (trimmedName is null)
        {
            throw new UserFriendlyException("工序名称不能为空");
        }

        Type = type;
        Code = trimmedCode;
        Name = trimmedName;
        Price = price;
        Cost = cost;
        IsCFUsable = isCFUsable;
        IsKCUsable = isKCUsable;
        Remark = remark;
    }

    /// <summary>
    /// 修改工序
    /// </summary>
    /// <param name="type">工序类型</param>
    /// <param name="code">工序代码</param>
    /// <param name="name">工序名称</param>
    /// <param name="price">价格</param>
    /// <param name="cost">成本</param>
    /// <param name="isCFUsable">车房可用</param>
    /// <param name="isKCUsable">库存可用</param>
    /// <param name="remark">备注(可空)</param>
    internal void Modify(string type, string code, string name, decimal price, decimal cost, bool isCFUsable, bool isKCUsable, string? remark)
    {
        string? trimmedCode = code?.Trim();
        string? trimmedName = name?.Trim();
        if (trimmedCode is null)
        {
            throw new UserFriendlyException("工序代码不能为空");
        }

        if (trimmedName is null)
        {
            throw new UserFriendlyException("工序名称不能为空");
        }

        Type = type;
        Code = trimmedCode;
        Name = trimmedName;
        Price = price;
        Cost = cost;
        IsCFUsable = isCFUsable;
        IsKCUsable = isKCUsable;
        Remark = remark;
    }


}
