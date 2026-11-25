using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.PurchasePlans;

/// <summary>
/// 采购计划领域服务
/// </summary>
[ScopedService]
public class PurchasePlanDomainService : IPurchasePlanDomainService
{
    /// <summary>
    /// SKU仓储
    /// </summary>
    private readonly ISkuRepository _skuRepository;

    /// <summary>
    /// 产品仓储
    /// </summary>
    private readonly ISpuRepository _spuRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="skuRepository">SKU仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    public PurchasePlanDomainService(ISkuRepository skuRepository, ISpuRepository spuRepository)
    {
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
    }

    /// <inheritdoc />
    public async Task SyncPurchasePlanItemsAsync(PurchasePlan purchasePlan, IReadOnlyCollection<PurchasePlanItemData>? skuInfos, CancellationToken cancellationToken = default)
    {
        List<PurchasePlanItemData> target = [];
        if (skuInfos is not null)
        {
            target = skuInfos.ToList();
        }

        List<PurchasePlanItem> toRemoves = purchasePlan.PurchasePlanItems.Where(a => !target.Any(i => i.SkuId == a.SkuId && i.WarehouseId == a.WarehouseId)).ToList();
        foreach (PurchasePlanItem item in toRemoves)
        {
            purchasePlan.DeleteItem(item.Id);
        }

        Dictionary<ItemKey, long> existingDict = new Dictionary<ItemKey, long>();
        foreach (PurchasePlanItem item in purchasePlan.PurchasePlanItems)
        {
            existingDict.TryAdd(new ItemKey(item.SkuId, item.WarehouseId), item.Id);
        }

        foreach (PurchasePlanItemData item in target)
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

            ItemKey key = new ItemKey(item.SkuId, item.WarehouseId);
            if (existingDict.TryGetValue(key, out long existingId))
            {
                purchasePlan.DeleteItem(existingId);
            }

            if (item.SalesSpuId is not null && !string.IsNullOrWhiteSpace(item.SalesSpuCode))
            {
                purchasePlan.AddItem(item.RelationItemId, item.SalesSpuId.Value, item.SalesSpuCode, item.CustomerSpuId, item.CustomerSpuCode, sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, sku.KeyAttributes, item.OtherAttributes);
            }
            else
            {
                purchasePlan.AddItem(item.RelationItemId, sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, sku.KeyAttributes, item.OtherAttributes);
            }
        }
    }

    /// <summary>
    /// 复合业务键
    /// </summary>
    /// <param name="SkuId">SKU Id</param>
    /// <param name="WarehouseId">仓库Id</param>
    private sealed record ItemKey(long SkuId, long WarehouseId);
}
