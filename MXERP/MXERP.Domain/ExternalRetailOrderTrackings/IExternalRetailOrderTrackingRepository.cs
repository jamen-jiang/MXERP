using Snow.Domain.Repositories;

namespace MXERP.Domain.ExternalRetailOrderTrackings;

/// <summary>
/// 外部散镜订单跟踪仓储接口
/// </summary>
public interface IExternalRetailOrderTrackingRepository : IRepository<ExternalRetailOrderTracking>
{
    /// <summary>
    /// 按外部散镜订单号 + 工序Id查询（用于重复录入校验）, 不存在返回 null
    /// </summary>
    /// <param name="number">外部散镜订单号</param>
    /// <param name="workStepId">工序Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>外部散镜订单跟踪, 不存在返回 null</returns>
    Task<ExternalRetailOrderTracking?> GetByNumberAndWorkStepIdAsync(string number, long workStepId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按外部散镜订单号查询跟踪列表（用于跳过工序校验）
    /// </summary>
    /// <param name="number">外部散镜订单号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>外部散镜订单跟踪列表</returns>
    Task<List<ExternalRetailOrderTracking>> GetListByNumberAsync(string number, CancellationToken cancellationToken = default);
}
