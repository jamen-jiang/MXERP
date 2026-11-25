namespace MXERP.Domain.ExternalRetailOrderTrackings;

/// <summary>
/// 外部散镜订单跟踪领域服务接口
/// </summary>
public interface IExternalRetailOrderTrackingDomainService
{
    /// <summary>
    /// 添加跟踪
    /// </summary>
    /// <param name="number">外部散镜订单号</param>
    /// <param name="workStepId">工序Id（可空）</param>
    /// <param name="reason">原因（可空）</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="inDepartmentId">入部门Id（当前用户关联部门）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>外部散镜订单跟踪, 工序与原因均未提供时返回 null</returns>
    Task<ExternalRetailOrderTracking?> AddTrackingAsync(string number, long? workStepId, string? reason, long employeeId, long inDepartmentId, CancellationToken cancellationToken = default);
}
