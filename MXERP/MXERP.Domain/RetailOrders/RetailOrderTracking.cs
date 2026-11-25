using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单跟踪(对应 Java RetailOrderTracking)
/// </summary>
public class RetailOrderTracking : CreationAuditedEntity
{
    /// <summary>
    /// 散镜订单Id
    /// </summary>
    public long? RetailOrderId { get; private set; }
    /// <summary>
    /// 原因(可空)
    /// </summary>
    public string? Reason { get; private set; }
    /// <summary>
    /// 工序Id(可空)
    /// </summary>
    public long? WorkStepId { get; private set; }
    /// <summary>
    /// 员工Id(可空)
    /// </summary>
    public long? EmployeeId { get; private set; }
    /// <summary>
    /// 出部门Id(可空)
    /// </summary>
    public long? OutDepartmentId { get; private set; }
    /// <summary>
    /// 入部门Id(可空)
    /// </summary>
    public long? InDepartmentId { get; private set; }
    /// <summary>
    /// 是否同步到HkoTracking
    /// </summary>
    public bool IsSyncHkoTracking { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected RetailOrderTracking()
    {
    }

    /// <summary>
    /// 构造器(按工序)
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal RetailOrderTracking(long workStepId, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        WorkStepId = workStepId;
        EmployeeId = employeeId;
        OutDepartmentId = outDepartmentId;
        InDepartmentId = inDepartmentId;
    }

    /// <summary>
    /// 构造器(按原因)
    /// </summary>
    /// <param name="reason">原因(可空)</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal RetailOrderTracking(string? reason, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        Reason = reason;
        EmployeeId = employeeId;
        OutDepartmentId = outDepartmentId;
        InDepartmentId = inDepartmentId;
    }

    /// <summary>
    /// 同步 HKO 跟踪
    /// </summary>
    public void SyncHkoTracking()
    {
        IsSyncHkoTracking = true;
    }
}
