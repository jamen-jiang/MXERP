using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.ExternalRetailOrderTrackings;

/// <summary>
/// 外部散镜订单跟踪
/// </summary>
public class ExternalRetailOrderTracking : CreationAuditedAggregateRoot
{
    /// <summary>
    /// 外部散镜订单号
    /// </summary>
    public string ExternalRetailOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 原因（可空）
    /// </summary>
    public string? Reason { get; private set; }
    /// <summary>
    /// 工序Id（可空）
    /// </summary>
    public long? WorkStepId { get; private set; }
    /// <summary>
    /// 员工Id
    /// </summary>
    public long EmployeeId { get; private set; }
    /// <summary>
    /// Out部门Id
    /// </summary>
    public long OutDepartmentId { get; private set; }
    /// <summary>
    /// In部门Id
    /// </summary>
    public long InDepartmentId { get; private set; }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected ExternalRetailOrderTracking()
    {
    }

    /// <summary>
    /// 构造器（按工序）
    /// </summary>
    /// <param name="externalRetailOrderNumber">外部散镜订单号</param>
    /// <param name="workStepId">工序Id</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal ExternalRetailOrderTracking(string externalRetailOrderNumber, long workStepId, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        ExternalRetailOrderNumber = externalRetailOrderNumber;
        WorkStepId = workStepId;
        EmployeeId = employeeId;
        OutDepartmentId = outDepartmentId;
        InDepartmentId = inDepartmentId;
    }

    /// <summary>
    /// 构造器（按原因）
    /// </summary>
    /// <param name="externalRetailOrderNumber">外部散镜订单号</param>
    /// <param name="reason">原因（可空）</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal ExternalRetailOrderTracking(string externalRetailOrderNumber, string? reason, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        ExternalRetailOrderNumber = externalRetailOrderNumber;
        Reason = reason;
        EmployeeId = employeeId;
        OutDepartmentId = outDepartmentId;
        InDepartmentId = inDepartmentId;
    }
}
