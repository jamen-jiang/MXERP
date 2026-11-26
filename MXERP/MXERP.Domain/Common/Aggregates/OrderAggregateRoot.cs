using MXERP.Domain.AuditLogs;
using MXERP.Domain.SerialNumbers;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Common.Aggregates;

/// <summary>
/// 订单聚合根基类(对应 Java OrderAggregateRoot)
/// </summary>
public abstract class OrderAggregateRoot : AuditedAggregateRoot
{
    /// <summary>
    /// 单号
    /// </summary>
    public string Number { get; private set; } = string.Empty;

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected OrderAggregateRoot()
    {
    }

    /// <summary>
    /// 业务创建入口: 必须带单号
    /// </summary>
    /// <param name="number">单号</param>
    protected OrderAggregateRoot(string number)
    {
        AssignNumber(number);
    }

    /// <summary>
    /// 获取流水号类型
    /// </summary>
    /// <returns>流水号类型</returns>
    public abstract string GetSerialNumberType();

    /// <summary>
    /// 分配单号, 只能分配一次
    /// </summary>
    /// <param name="number">单号</param>
    public virtual void AssignNumber(string number)
    {
        if (!string.IsNullOrWhiteSpace(Number))
        {
            throw new UserFriendlyException("已分配单号，不能重复分配!");
        }

        Number = number;
        AddDomainEvent(new OrderAddedEvent(this, GetSerialNumberType()));
    }

    /// <summary>
    /// 记录审计日志
    /// </summary>
    /// <param name="action">动作</param>
    public void Audit(string action)
    {
        Audit(action, null);
    }

    /// <summary>
    /// 记录审计日志
    /// </summary>
    /// <param name="action">动作</param>
    /// <param name="remark">备注(可空)</param>
    public void Audit(string action, string? remark)
    {
        AddDomainEvent(new AddAuditLogEvent(this, action, remark));
    }
}
