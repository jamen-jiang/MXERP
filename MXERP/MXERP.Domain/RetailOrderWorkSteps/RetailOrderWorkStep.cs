using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.RetailOrderWorkSteps;

/// <summary>
/// 散镜订单工作步骤
/// </summary>
public class RetailOrderWorkStep : AuditedAggregateRoot
{
    /// <summary>
    /// 组织机构Id
    /// </summary>
    public long DepartmentId { get; private set; }

    /// <summary>
    /// 序列
    /// </summary>
    public int Sequence { get; private set; }

    /// <summary>
    /// 代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected RetailOrderWorkStep()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="departmentId">组织机构Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注（可空）</param>
    internal RetailOrderWorkStep(long departmentId, string code, string name, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("散镜订单工作步骤代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("散镜订单工作步骤名称不能为空");
        }

        DepartmentId = departmentId;
        Code = code.Trim();
        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 修改散镜订单工作步骤
    /// </summary>
    /// <param name="departmentId">组织机构Id</param>
    /// <param name="name">名称</param>
    /// <param name="remark">备注（可空）</param>
    internal void Modify(long departmentId, string name, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("散镜订单工作步骤名称不能为空");
        }

        DepartmentId = departmentId;
        Name = name.Trim();
        Remark = remark;
    }

    /// <summary>
    /// 修改序列
    /// </summary>
    /// <param name="sequence">序列</param>
    internal void ChangeSequence(int sequence)
    {
        Sequence = sequence;
    }
}
