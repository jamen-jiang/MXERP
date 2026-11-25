namespace MXERP.Domain.SalesSpuOtherAssignItems;

/// <summary>
/// 销售产品其他分配项工厂接口
/// </summary>
public interface ISalesSpuOtherAssignItemFactory
{
    /// <summary>
    /// 生成销售产品其他分配项
    /// </summary>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="coating">焗色</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>销售产品其他分配项聚合</returns>
    Task<SalesSpuOtherAssignItem> GenerateAsync(long salesSpuId, string coating, long spuId, long warehouseId, CancellationToken cancellationToken = default);
}
