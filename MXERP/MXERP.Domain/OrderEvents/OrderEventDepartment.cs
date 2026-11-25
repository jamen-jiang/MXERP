using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.OrderEvents;

/// <summary>
/// 订单事件绑定的组织机构
/// </summary>
public class OrderEventDepartment : DeletionAuditedEntity
{
    /// <summary>
    /// 订单事件Id
    /// </summary>
    public long? OrderEventId { get; private set; }

    /// <summary>
    /// 组织机构Id
    /// </summary>
    public long DepartmentId { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected OrderEventDepartment()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="departmentId">组织机构Id</param>
    internal OrderEventDepartment(long departmentId)
    {
        DepartmentId = departmentId;
    }
}
