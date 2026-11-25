namespace MXERP.Domain.PackingSlips;

/// <summary>
/// 装箱单领域服务接口
/// </summary>
public interface IPackingSlipDomainService
{
    /// <summary>
    /// 创建装箱单
    /// </summary>
    /// <param name="data">装箱单数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>装箱单聚合</returns>
    Task<PackingSlip> CreateAsync(PackingSlipData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成：装箱单存在则更新，不存在则生成
    /// </summary>
    /// <param name="data">装箱单数据</param>
    /// <param name="currentUserId">当前用户Id</param>
    /// <param name="currentUserName">当前用户姓名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>装箱单聚合</returns>
    Task<PackingSlip> ImportGenerateAsync(PackingSlipData data, long currentUserId, string currentUserName, CancellationToken cancellationToken = default);
}
