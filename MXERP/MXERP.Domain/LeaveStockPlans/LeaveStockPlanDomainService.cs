using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockPlans;

/// <summary>
/// 出库计划领域服务
/// </summary>
[ScopedService]
public class LeaveStockPlanDomainService : ILeaveStockPlanDomainService
{
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="skuRepository">SKU仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    public LeaveStockPlanDomainService(ISkuRepository skuRepository, ISpuRepository spuRepository)
    {
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
    }

    /// <inheritdoc />
    public async Task SyncLeaveStockPlanItemsAsync(LeaveStockPlan leaveStockPlan, IReadOnlyCollection<LeaveStockPlanItemData>? skuInfos, CancellationToken cancellationToken = default)
    {
        List<LeaveStockPlanItemData> target = skuInfos is null ? [] : skuInfos.ToList();

        List<LeaveStockPlanItem> toRemoves = leaveStockPlan.LeaveStockPlanItems.Where(a => !target.Any(i => i.SalesSpuId == a.SalesSpuId && i.SkuId == a.SkuId && i.WarehouseId == a.WarehouseId)).ToList();
        foreach (LeaveStockPlanItem item in toRemoves)
        {
            leaveStockPlan.DeleteItem(item.Id);
        }

        Dictionary<ItemKey, long> existingDict = new Dictionary<ItemKey, long>();
        foreach (LeaveStockPlanItem item in leaveStockPlan.LeaveStockPlanItems)
        {
            existingDict.TryAdd(new ItemKey(item.SalesSpuId, item.SkuId, item.WarehouseId), item.Id);
        }

        foreach (LeaveStockPlanItemData item in target)
        {
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku is null)
            {
                throw new UserFriendlyException("产品Sku " + item.SkuId + "不存在!");
            }

            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
            if (spu is null)
            {
                throw new UserFriendlyException("产品 " + sku.SpuId + "不存在!");
            }

            ItemKey key = new ItemKey(item.SalesSpuId, item.SkuId, item.WarehouseId);
            if (existingDict.TryGetValue(key, out long existingId))
            {
                leaveStockPlan.DeleteItem(existingId);
            }

            if (item.SalesSpuId is not null && !string.IsNullOrWhiteSpace(item.SalesSpuCode))
            {
                leaveStockPlan.AddItem(item.RelationItemId, item.SalesSpuId.Value, item.SalesSpuCode, item.CustomerSpuId, item.CustomerSpuCode, sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, sku.KeyAttributes, item.OtherAttributes);
            }
            else
            {
                leaveStockPlan.AddItem(item.RelationItemId, sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, sku.KeyAttributes, item.OtherAttributes);
            }
        }
    }

    /// <summary>
    /// 复合业务键
    /// </summary>
    /// <param name="SalesSpuId">销售产品Id（可空）</param>
    /// <param name="SkuId">SKU Id</param>
    /// <param name="WarehouseId">仓库Id</param>
    private sealed record ItemKey(long? SalesSpuId, long SkuId, long WarehouseId);
}
