namespace MXERP.Domain.SalesSpuOtherAssignItems;

/// <summary>
/// 销售产品其他分配项领域服务接口
/// </summary>
public interface ISalesSpuOtherAssignItemDomainService
{
    /// <summary>
    /// 修改销售产品其他分配项(按Id查询后修改)
    /// </summary>
    /// <param name="id">销售产品其他分配项Id</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品其他分配项聚合</returns>
    Task<SalesSpuOtherAssignItem> ModifyAsync(long id, long salesSpuId, string coating, long spuId, long warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改销售产品其他分配项(使用已有聚合)
    /// </summary>
    /// <param name="model">销售产品其他分配项聚合</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task ModifyAsync(SalesSpuOtherAssignItem model, long salesSpuId, string coating, long spuId, long warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 导入生成(存在则修改, 不存在则创建)
    /// </summary>
    /// <param name="companyCode">公司代码</param>
    /// <param name="salesCode">销售产品代码</param>
    /// <param name="coating">焗色</param>
    /// <param name="spuCode">产品代码</param>
    /// <param name="warehouseCode">仓库代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品其他分配项聚合</returns>
    Task<SalesSpuOtherAssignItem> ImportGenerateAsync(string companyCode, string salesCode, string coating, string spuCode, string warehouseCode, CancellationToken cancellationToken = default);
}
