using MXERP.Domain.Common.Contracts;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.InventoryOrders;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.PurchaseOrders;
using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单领域服务
/// </summary>
[ScopedService]
public class EnterStockOrderDomainService : IEnterStockOrderDomainService
{
    private readonly IEnterStockOrderRepository _enterStockOrderRepository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;
    private readonly IInventoryOrderRepository _inventoryOrderRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly IEnterStockOrderFactory _enterStockOrderFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="enterStockOrderRepository">入库单仓储</param>
    /// <param name="purchaseOrderRepository">采购单仓储</param>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    /// <param name="inventoryOrderRepository">盘点单仓储</param>
    /// <param name="skuRepository">产品Sku仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="enterStockOrderFactory">入库单工厂</param>
    public EnterStockOrderDomainService(IEnterStockOrderRepository enterStockOrderRepository, IPurchaseOrderRepository purchaseOrderRepository, ILeaveStockBatchRepository leaveStockBatchRepository, IInventoryOrderRepository inventoryOrderRepository, ISkuRepository skuRepository, ISpuRepository spuRepository, IEnterStockOrderFactory enterStockOrderFactory)
    {
        _enterStockOrderRepository = enterStockOrderRepository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _leaveStockBatchRepository = leaveStockBatchRepository;
        _inventoryOrderRepository = inventoryOrderRepository;
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _enterStockOrderFactory = enterStockOrderFactory;
    }

    /// <inheritdoc />
    public async Task SyncEnterStockOrderItemsAsync(EnterStockOrder enterStockOrder, IReadOnlyCollection<SkuInfoData>? skuInfos, CancellationToken cancellationToken = default)
    {
        List<SkuInfoData> finalSkuInfos = new List<SkuInfoData>();
        if (skuInfos != null)
        {
            finalSkuInfos = skuInfos.Where(x => x.Qty != 0).ToList();
        }

        // 1. 删除未在输入中出现的属性（通过主键）
        List<EnterStockOrderItem> toRemoves = enterStockOrder.GetEnterStockOrderItems()
            .Where(a => !finalSkuInfos.Any(i => i.SkuId == a.SkuId && i.WarehouseId == a.WarehouseId))
            .ToList();
        foreach (EnterStockOrderItem item in toRemoves)
        {
            enterStockOrder.DeleteItem(item.Id);
        }

        // 2. 建立"业务标识 → 已持久化属性主键"的映射（仅用于已存在的属性）
        Dictionary<SkuWarehouse, long> existingDict = new Dictionary<SkuWarehouse, long>();
        foreach (EnterStockOrderItem a in enterStockOrder.GetEnterStockOrderItems())
        {
            existingDict[new SkuWarehouse(a.SkuId, a.WarehouseId)] = a.Id;
        }

        foreach (SkuInfoData item in finalSkuInfos)
        {
            // 3. 查产品Sku
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, false, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku {item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, false, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品 {sku.SpuId}不存在!");
            }

            // 4. 更新：存在的
            SkuWarehouse key = new SkuWarehouse(item.SkuId, item.WarehouseId);
            if (existingDict.TryGetValue(key, out long id))
            {
                enterStockOrder.ModifyItem(id, item.Qty);
            }
            else
            {
                // 5. 添加：不存在的
                enterStockOrder.AddItem(sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, sku.PurchasePrice ?? spu.PurchasePrice, sku.KeyAttributes, null);
            }
        }
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder> GenerateAsync(long enterStockOrderId, string? remark, IReadOnlyCollection<SkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查原入库单
        EnterStockOrder? originalEnterStockOrder = await _enterStockOrderRepository.GetByIdAsync(enterStockOrderId, cancellationToken);
        if (originalEnterStockOrder == null)
        {
            throw new UserFriendlyException("入库单不存在!");
        }
        if (EnterStockOrderStatusEnum.Finished != originalEnterStockOrder.Status)
        {
            throw new UserFriendlyException($"入库单{originalEnterStockOrder.Number}当前状态不允许冲销!");
        }

        // 2. 生成红冲入库单（委托工厂，含发号）
        EnterStockOrder model;
        if (string.IsNullOrWhiteSpace(originalEnterStockOrder.RelationType))
        {
            model = await _enterStockOrderFactory.CreateAsync(originalEnterStockOrder.ProductType, enterStockOrderId, EnterStockBusinessTypeEnum.ReversalEnterStock, remark, cancellationToken);
        }
        else
        {
            model = await _enterStockOrderFactory.CreateAsync(originalEnterStockOrder.ProductType, enterStockOrderId, originalEnterStockOrder.RelationType, EnterStockBusinessTypeEnum.ReversalEnterStock, remark, cancellationToken);
        }

        // 3. 添加明细
        foreach (SkuInfoData item in skuInfos.Where(x => x.Qty != 0))
        {
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, false, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku{item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, false, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品{sku.SpuId}不存在!");
            }
            EnterStockOrderItem? enterStockOrderItem = originalEnterStockOrder.GetEnterStockOrderItems()
                .FirstOrDefault(x => x.SkuId == item.SkuId && x.WarehouseId == item.WarehouseId);
            if (enterStockOrderItem == null)
            {
                throw new UserFriendlyException("入库单项不存在!");
            }

            if (!string.IsNullOrWhiteSpace(originalEnterStockOrder.RelationType))
            {
                // 3.1 有关联：按关联项拆分冲销数量
                double remainingToReverse = Math.Abs(item.Qty);
                foreach (EnterStockOrderRelation relation in enterStockOrderItem.GetEnterStockOrderRelations())
                {
                    double reverseQty = Math.Min(remainingToReverse, relation.Qty);
                    if (reverseQty > 0)
                    {
                        model.AddItem(enterStockOrderItem.Id, relation.RelationType, relation.RelationId, relation.RelationItemId, relation.RelationNumber, sku.SpuId, sku.Id, item.WarehouseId, -reverseQty, sku.PurchasePrice ?? spu.PurchasePrice, enterStockOrderItem.SkuAttributes, enterStockOrderItem.OtherAttributes);
                    }
                    remainingToReverse -= reverseQty;
                    if (remainingToReverse == 0)
                    {
                        break;
                    }
                }
                if (remainingToReverse > 0)
                {
                    throw new UserFriendlyException("冲销数量超过原入库数量!");
                }
            }
            else
            {
                // 3.2 无关联：直接添加
                model.AddItem(enterStockOrderItem.Id, sku.SpuId, sku.Id, item.WarehouseId, item.Qty, sku.PurchasePrice ?? spu.PurchasePrice, enterStockOrderItem.SkuAttributes, enterStockOrderItem.OtherAttributes);
            }
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder> GenerateFromPurchaseOrderAsync(long purchaseOrderId, IReadOnlyCollection<UniqueSkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查采购单
        PurchaseOrder? purchaseOrder = await _purchaseOrderRepository.FindAsync(purchaseOrderId, true, cancellationToken);
        if (purchaseOrder == null)
        {
            throw new UserFriendlyException($"采购单{purchaseOrderId}不存在!");
        }

        // 2. 生成入库单（委托工厂）
        string relationType = BusinessOrderTypeEnum.PurchaseOrder;
        EnterStockOrder model = await _enterStockOrderFactory.CreateAsync(purchaseOrder.ProductType, null, relationType, EnterStockBusinessTypeEnum.PurchaseEnterStock, purchaseOrder.Remark, cancellationToken);

        // 3. 添加明细
        foreach (UniqueSkuInfoData item in skuInfos.Where(x => x.Qty != 0))
        {
            // 3.1 查采购单项
            PurchaseOrderItem? purchaseOrderItem = purchaseOrder.GetPurchaseOrderItems().FirstOrDefault(x => x.Id == item.Id);
            if (purchaseOrderItem == null)
            {
                throw new UserFriendlyException("采购单项不存在!");
            }

            // 3.2 查产品Sku、产品
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, false, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku {item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, false, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品{sku.SpuId}不存在!");
            }

            // 3.3 添加入库单明细
            model.AddItem(relationType, purchaseOrder.Id, purchaseOrderItem.Id, purchaseOrder.Number, sku.SpuId, sku.Id, item.WarehouseId, item.Qty, sku.PurchasePrice ?? spu.PurchasePrice, purchaseOrderItem.SkuAttributes, null);
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder?> GenerateFromInventoryOrderAsync(long inventoryOrderId, long userId, string userName, CancellationToken cancellationToken = default)
    {
        InventoryOrder? inventoryOrder = await _inventoryOrderRepository.FindAsync(inventoryOrderId, true, cancellationToken);
        if (inventoryOrder == null)
        {
            throw new UserFriendlyException($"盘点单{inventoryOrderId}不存在!");
        }

        if (inventoryOrder.ProfitQty <= 0)
        {
            return null;
        }

        // 1. 生成入库单（委托工厂）
        string relationType = BusinessOrderTypeEnum.InventoryOrder;
        string businessType = EnterStockBusinessTypeEnum.InventoryEnterStock;

        EnterStockOrder model = await _enterStockOrderFactory.CreateAsync(inventoryOrder.ProductType, null, relationType, businessType, inventoryOrder.Remark, cancellationToken);

        // 2. 添加明细
        foreach (InventoryOrderItem item in inventoryOrder.GetInventoryOrderItems().Where(x => x.ProfitQty > 0))
        {
            // 2.1 查产品Sku、产品
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, false, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku {item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, false, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品 {sku.SpuId}不存在!");
            }

            // 2.2 添加入库单明细
            model.AddItem(relationType, inventoryOrder.Id, item.Id, inventoryOrder.Number, item.SpuId, item.SkuId, item.WarehouseId, item.ProfitQty, sku.PurchasePrice ?? spu.PurchasePrice, item.SkuAttributes, null);
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder> GenerateFromLeaveStockBatchAsync(long leaveStockBatchId, string businessType, string remark, IReadOnlyCollection<SkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查出库批次
        LeaveStockBatch? leaveStockBatch = await _leaveStockBatchRepository.FindAsync(leaveStockBatchId, false, cancellationToken);
        if (leaveStockBatch == null)
        {
            throw new UserFriendlyException($"出库批次{leaveStockBatchId}不存在!");
        }
        if (leaveStockBatch.PurchaseBatchId == null)
        {
            throw new UserFriendlyException($"出库批次{leaveStockBatch.Number}不允许生成入库单!");
        }
        if (LeaveStockBatchStatusEnum.Pending != leaveStockBatch.Status)
        {
            throw new UserFriendlyException($"出库批次{leaveStockBatch.Number}当前状态不允许生成入库单!");
        }
        if (string.IsNullOrWhiteSpace(remark))
        {
            throw new UserFriendlyException("备注为空不允许生成入库单!");
        }

        // 2. 生成入库单（委托工厂，无 businessOrderType）
        EnterStockOrder model = await _enterStockOrderFactory.CreateAsync(leaveStockBatch.ProductType, null, businessType, remark, cancellationToken);

        // 3. 添加明细
        foreach (SkuInfoData item in skuInfos.Where(x => x.Qty != 0))
        {
            // 3.1 查产品Sku、产品
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, false, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku{item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, false, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品{sku.SpuId}不存在!");
            }

            // 3.2 添加入库单明细
            model.AddItem(sku.SpuId, sku.Id, item.WarehouseId, item.Qty, sku.PurchasePrice ?? spu.PurchasePrice, sku.KeyAttributes, null);
        }

        // 4. 完成入库单
        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<EnterStockOrder?> UpdateReversalQtyAsync(long reversalEnterStockOrderId, CancellationToken cancellationToken = default)
    {
        // 1. 查红冲入库单
        EnterStockOrder? reversalEnterStockOrder = await _enterStockOrderRepository.GetByIdAsync(reversalEnterStockOrderId, cancellationToken);
        if (reversalEnterStockOrder == null)
        {
            throw new UserFriendlyException($"入库单{reversalEnterStockOrderId}不存在!");
        }
        if (reversalEnterStockOrder.OriginalId == null)
        {
            return null;
        }

        // 2. 查原入库单
        EnterStockOrder? model = await _enterStockOrderRepository.GetByIdAsync(reversalEnterStockOrder.OriginalId.Value, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("原入库单不存在!");
        }

        // 3. 累加冲销数量
        foreach (EnterStockOrderItem item in reversalEnterStockOrder.GetEnterStockOrderItems())
        {
            if (item.OriginalId == null)
            {
                throw new UserFriendlyException($"入库单项{item.Id}不存在原入库单项Id!");
            }
            model.UpdateReversalQty(item.OriginalId.Value, Math.Abs(item.Qty));
        }
        return model;
    }

    /// <summary>
    /// Sku + 仓库组合（用于去重）
    /// </summary>
    /// <param name="SkuId">产品SkuId</param>
    /// <param name="WarehouseId">仓库Id</param>
    private sealed record SkuWarehouse(long SkuId, long WarehouseId);
}
