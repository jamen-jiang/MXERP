using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.SalesSpuOtherAssignItems;

/// <summary>
/// 销售产品其他分配项工厂
/// </summary>
[ScopedService]
public class SalesSpuOtherAssignItemFactory : ISalesSpuOtherAssignItemFactory
{
    private readonly ISalesSpuOtherAssignItemRepository _salesSpuOtherAssignItemRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="salesSpuOtherAssignItemRepository">销售产品其他分配项仓储</param>
    public SalesSpuOtherAssignItemFactory(ISalesSpuOtherAssignItemRepository salesSpuOtherAssignItemRepository)
    {
        _salesSpuOtherAssignItemRepository = salesSpuOtherAssignItemRepository;
    }

    /// <inheritdoc />
    public async Task<SalesSpuOtherAssignItem> GenerateAsync(long salesSpuId, string coating, long spuId, long warehouseId, CancellationToken cancellationToken = default)
    {
        if (await _salesSpuOtherAssignItemRepository.IsExistBySalesSpuIdAndCoatingExcludeIdAsync(salesSpuId, coating, null, cancellationToken))
        {
            throw new UserFriendlyException("销售产品其他分配项已存在!");
        }

        return new SalesSpuOtherAssignItem(salesSpuId, coating, spuId, warehouseId);
    }
}
