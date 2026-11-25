using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.WorkSteps;

/// <summary>
/// 工作步骤(对应 Java WorkStep, 继承 UpdateAggregateRoot)
/// </summary>
public class WorkStep : AuditedAggregateRoot
{
    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; private set; }
    /// <summary>
    /// 序号
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
    /// 是否必须
    /// </summary>
    public bool IsMust { get; private set; }
    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    private WorkStep()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="departmentId">部门Id</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="isMust">是否必须</param>
    /// <param name="remark">备注</param>
    internal WorkStep(long departmentId, string code, string name, bool isMust, string? remark)
    {
        if (code.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("工作步骤代码不能为空");
        }

        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("工作步骤名称不能为空");
        }

        DepartmentId = departmentId;
        Code = code.Trim();
        Name = name.Trim();
        IsMust = isMust;
        Remark = remark;
    }

    /// <summary>
    /// 修改工序
    /// </summary>
    /// <param name="departmentId">部门Id</param>
    /// <param name="name">名称</param>
    /// <param name="isMust">是否必须</param>
    /// <param name="remark">备注</param>
    internal void Modify(long departmentId, string name, bool isMust, string? remark)
    {
        if (name.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("工作步骤名称不能为空");
        }

        DepartmentId = departmentId;
        Name = name.Trim();
        IsMust = isMust;
        Remark = remark;
    }

    /// <summary>
    /// ChangeSequence(工序)
    /// </summary>
    /// <param name="sequence">序号</param>
    internal void ChangeSequence(int sequence)
    {
        Sequence = sequence;
    }
}
