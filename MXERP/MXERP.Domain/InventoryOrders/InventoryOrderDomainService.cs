using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.Common.Contracts;
using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using MXERP.Domain.Stocks;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单领域服务
/// </summary>
[ScopedService]
public class InventoryOrderDomainService : IInventoryOrderDomainService
{
    private readonly IInventoryOrderRepository _inventoryOrderRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly IStockRepository _stockRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="inventoryOrderRepository">盘点单仓储</param>
    /// <param name="skuRepository">产品Sku仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="stockRepository">库存仓储</param>
    public InventoryOrderDomainService(
        IInventoryOrderRepository inventoryOrderRepository,
        ISkuRepository skuRepository,
        ISpuRepository spuRepository,
        IStockRepository stockRepository)
    {
        _inventoryOrderRepository = inventoryOrderRepository;
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _stockRepository = stockRepository;
    }

    /// <inheritdoc />
    public async Task SyncInventoryOrderItemsAsync(InventoryOrder inventoryOrder, IReadOnlyCollection<SkuInfoData>? skuInfos, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<SkuInfoData> finalSkuInfos = skuInfos ?? new List<SkuInfoData>();

        List<InventoryOrderItem> toRemoves = inventoryOrder.GetInventoryOrderItems()
            .Where(a => !finalSkuInfos.Any(i => i.SkuId == a.SkuId && i.WarehouseId == a.WarehouseId))
            .ToList();
        foreach (InventoryOrderItem item in toRemoves)
        {
            inventoryOrder.DeleteItem(item.Id);
        }

        Dictionary<SkuWarehouse, long> existingDict = new Dictionary<SkuWarehouse, long>();
        foreach (InventoryOrderItem a in inventoryOrder.GetInventoryOrderItems())
        {
            existingDict[new SkuWarehouse(a.SkuId, a.WarehouseId)] = a.Id;
        }

        foreach (SkuInfoData item in finalSkuInfos)
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

            SkuWarehouse key = new SkuWarehouse(item.SkuId, item.WarehouseId);
            if (existingDict.TryGetValue(key, out long id))
            {
                inventoryOrder.ModifyItem(id, item.Qty);
            }
            else
            {
                inventoryOrder.AddItem(sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, sku.KeyAttributes, null);
            }
        }
    }

    /// <inheritdoc />
    public async Task<InventoryOrder> CheckAsync(long id, long userId, string name, CancellationToken cancellationToken = default)
    {
        InventoryOrder? model = await _inventoryOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("盘点单:" + id + "不存在!");
        }

        model.Check(userId, name);
        foreach (InventoryOrderItem item in model.GetInventoryOrderItems())
        {
            Stock? stock = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(item.SkuId, item.WarehouseId, cancellationToken);
            double stockQty = 0;
            if (stock != null)
            {
                stockQty = stock.Qty;
            }

            model.TaskStock(item.SkuId, item.WarehouseId, stockQty);
        }

        return model;
    }

    /// <summary>
    /// Sku + 仓库组合(用于去重)
    /// </summary>
    /// <param name="SkuId">产品SkuId</param>
    /// <param name="WarehouseId">仓库Id</param>
    private sealed record SkuWarehouse(long SkuId, long WarehouseId);
}
