using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.LeaveStockBatches;

/// <summary>
/// 出库批次跟踪
/// </summary>
public class LeaveStockBatchTracking : CreationAuditedEntity
{
    /// <summary>
    /// 出库批次Id
    /// </summary>
    public long? LeaveStockBatchId { get; private set; }

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
    /// 出部门Id
    /// </summary>
    public long OutDepartmentId { get; private set; }

    /// <summary>
    /// 入部门Id
    /// </summary>
    public long InDepartmentId { get; private set; }

    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected LeaveStockBatchTracking()
    {
    }

    /// <summary>
    /// 构造器（按工序）
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal LeaveStockBatchTracking(long workStepId, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        WorkStepId = workStepId;
        EmployeeId = employeeId;
        OutDepartmentId = outDepartmentId;
        InDepartmentId = inDepartmentId;
    }

    /// <summary>
    /// 构造器（按原因）
    /// </summary>
    /// <param name="reason">原因（可空）</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal LeaveStockBatchTracking(string? reason, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        Reason = reason;
        EmployeeId = employeeId;
        OutDepartmentId = outDepartmentId;
        InDepartmentId = inDepartmentId;
    }
}
