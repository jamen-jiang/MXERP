namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单领域服务接口
/// </summary>
public interface IRetailOrderDomainService
{
    /// <summary>
    /// 创建散镜订单
    /// </summary>
    /// <param name="data">散镜订单数据</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> CreateAsync(RetailOrderData data, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改散镜订单
    /// </summary>
    /// <param name="data">散镜订单数据(必须带Id)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> ModifyAsync(RetailOrderData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// 弃审
    /// </summary>
    /// <param name="id">散镜订单Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> UnCheckAsync(long id, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 完成
    /// </summary>
    /// <param name="id">散镜订单Id</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> FinishAsync(long id, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 弃完
    /// </summary>
    /// <param name="id">散镜订单Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> UnFinishAsync(long id, string? remark, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="id">散镜订单Id</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> CancelAsync(long id, string? remark, long userId, string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加跟踪
    /// </summary>
    /// <param name="number">散镜订单号</param>
    /// <param name="workStepId">工序Id(可空)</param>
    /// <param name="reason">原因(可空)</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    /// <param name="userId">操作人Id</param>
    /// <param name="userName">操作人名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>散镜订单</returns>
    Task<RetailOrder> AddTrackingAsync(string number, long? workStepId, string? reason, long employeeId, long inDepartmentId, long userId, string userName, CancellationToken cancellationToken = default);
}
