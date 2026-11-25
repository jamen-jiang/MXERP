using MXERP.Domain.Common.Contracts;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Common.Utils;
using MXERP.Domain.InventoryOrders;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.RetailOrders;
using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单领域服务
/// </summary>
[ScopedService]
public class LeaveStockOrderDomainService : ILeaveStockOrderDomainService
{
    private readonly ILeaveStockOrderRepository _leaveStockOrderRepository;
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;
    private readonly IRetailOrderRepository _retailOrderRepository;
    private readonly IInventoryOrderRepository _inventoryOrderRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly ILeaveStockOrderFactory _leaveStockOrderFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveStockOrderRepository">出库单仓储</param>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    /// <param name="retailOrderRepository">散镜订单仓储</param>
    /// <param name="inventoryOrderRepository">盘点单仓储</param>
    /// <param name="skuRepository">产品Sku仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="leaveStockOrderFactory">出库单工厂</param>
    public LeaveStockOrderDomainService(ILeaveStockOrderRepository leaveStockOrderRepository, ILeaveStockBatchRepository leaveStockBatchRepository, IRetailOrderRepository retailOrderRepository, IInventoryOrderRepository inventoryOrderRepository, ISkuRepository skuRepository, ISpuRepository spuRepository, ILeaveStockOrderFactory leaveStockOrderFactory)
    {
        _leaveStockOrderRepository = leaveStockOrderRepository;
        _leaveStockBatchRepository = leaveStockBatchRepository;
        _retailOrderRepository = retailOrderRepository;
        _inventoryOrderRepository = inventoryOrderRepository;
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _leaveStockOrderFactory = leaveStockOrderFactory;
    }

    /// <inheritdoc />
    public async Task SyncLeaveStockOrderItemsAsync(LeaveStockOrder leaveStockOrder, IReadOnlyCollection<SkuInfoData>? skuInfos, CancellationToken cancellationToken = default)
    {
        List<SkuInfoData> finalSkuInfos = new List<SkuInfoData>();
        if (skuInfos != null)
        {
            finalSkuInfos = skuInfos.Where(x => x.Qty != 0).ToList();
        }

        // 1. 删除未在输入中出现的属性（通过主键）
        List<LeaveStockOrderItem> toRemoves = leaveStockOrder.LeaveStockOrderItems
            .Where(a => !finalSkuInfos.Any(i => i.SkuId == a.SkuId && i.WarehouseId == a.WarehouseId))
            .ToList();
        foreach (LeaveStockOrderItem item in toRemoves)
        {
            leaveStockOrder.DeleteItem(item.Id);
        }

        // 2. 建立"业务标识 → 已持久化属性主键"的映射（仅用于已存在的属性）
        Dictionary<SkuWarehouse, long> existingDict = new Dictionary<SkuWarehouse, long>();
        foreach (LeaveStockOrderItem a in leaveStockOrder.LeaveStockOrderItems)
        {
            existingDict[new SkuWarehouse(a.SkuId, a.WarehouseId)] = a.Id;
        }

        foreach (SkuInfoData item in finalSkuInfos)
        {
            // 3. 查产品Sku
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku {item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品 {sku.SpuId}不存在!");
            }

            // 4. 更新：存在的
            long existingId;
            bool exists = existingDict.TryGetValue(new SkuWarehouse(item.SkuId, item.WarehouseId), out existingId);
            if (exists)
            {
                leaveStockOrder.ModifyItem(existingId, item.Qty);
            }
            // 5. 添加：不存在的
            else
            {
                decimal price = spu.PurchasePrice;
                if (sku.PurchasePrice != null)
                {
                    price = sku.PurchasePrice.Value;
                }
                leaveStockOrder.AddItem(sku.SpuId, item.SkuId, item.WarehouseId, item.Qty, price, sku.KeyAttributes, null);
            }
        }
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder> GenerateAsync(long leaveStockOrderId, string? remark, IReadOnlyCollection<SkuInfoData> skuInfos, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查原出库单
        LeaveStockOrder? originalLeaveStockOrder = await _leaveStockOrderRepository.FindAsync(leaveStockOrderId, true, cancellationToken);
        if (originalLeaveStockOrder == null)
        {
            throw new UserFriendlyException("出库单不存在!");
        }
        if (LeaveStockOrderStatusEnum.Finished != originalLeaveStockOrder.Status)
        {
            throw new UserFriendlyException($"出库单{originalLeaveStockOrder.Number}当前状态不允许冲销!");
        }

        // 2. 生成红冲出库单（委托工厂，含发号）
        LeaveStockOrder model;
        if (originalLeaveStockOrder.RelationType is null)
        {
            model = await _leaveStockOrderFactory.CreateAsync(originalLeaveStockOrder.ProductType, leaveStockOrderId, LeaveStockBusinessTypeEnum.ReversalLeaveStock, remark, cancellationToken);
        }
        else
        {
            model = await _leaveStockOrderFactory.CreateAsync(originalLeaveStockOrder.ProductType, leaveStockOrderId, originalLeaveStockOrder.RelationType.Value, LeaveStockBusinessTypeEnum.ReversalLeaveStock, remark, cancellationToken);
        }

        // 3. 添加明细
        foreach (SkuInfoData item in skuInfos.Where(x => x.Qty != 0).ToList())
        {
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku{item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品{sku.SpuId}不存在!");
            }
            LeaveStockOrderItem? leaveStockOrderItem = originalLeaveStockOrder.LeaveStockOrderItems
                .FirstOrDefault(x => x.SkuId == item.SkuId && x.WarehouseId == item.WarehouseId);
            if (leaveStockOrderItem == null)
            {
                throw new UserFriendlyException("出库单项不存在!");
            }

            decimal price = spu.PurchasePrice;
            if (sku.PurchasePrice != null)
            {
                price = sku.PurchasePrice.Value;
            }

            if (originalLeaveStockOrder.RelationType is not null)
            {
                // 3.1 有关联：按关联项拆分红冲数量
                double remainingToReverse = Math.Abs(item.Qty);
                foreach (LeaveStockOrderRelation relation in leaveStockOrderItem.LeaveStockOrderRelations)
                {
                    double reverseQty = Math.Min(remainingToReverse, relation.Qty);
                    if (reverseQty > 0)
                    {
                        model.AddItem(leaveStockOrderItem.Id, relation.RelationType, relation.RelationId, relation.RelationItemId, relation.RelationNumber, sku.SpuId, sku.Id, item.WarehouseId, -reverseQty, price, leaveStockOrderItem.SkuAttributes, leaveStockOrderItem.OtherAttributes);
                    }
                    remainingToReverse -= reverseQty;
                    if (remainingToReverse == 0)
                    {
                        break;
                    }
                }
                if (remainingToReverse > 0)
                {
                    throw new UserFriendlyException("冲销数量超过原出库数量!");
                }
            }
            else
            {
                // 3.2 无关联：直接添加
                model.AddItem(leaveStockOrderItem.Id, sku.SpuId, sku.Id, item.WarehouseId, item.Qty, price, leaveStockOrderItem.SkuAttributes, leaveStockOrderItem.OtherAttributes);
            }
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder> GenerateFromLeaveStockBatchesAsync(IReadOnlyCollection<GenerateLeaveStockOrderFromBatchData> datas, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查出库批次列表
        List<long> batchIds = datas.Select(x => x.LeaveStockBatchId).ToList();
        List<LeaveStockBatch> batches = await _leaveStockBatchRepository.GetListByIdsAsync(batchIds, cancellationToken);
        if (batches.Count < batchIds.Count)
        {
            throw new UserFriendlyException("出库批次不存在!");
        }

        // 2. 校验抽仓 / 订购类型
        long deductCount = batches.Count(x => x.LeaveStockPlanId != null);
        long customizedCount = batches.Count(x => x.LeaveStockPlanId == null);
        if (deductCount > 0 && customizedCount > 0)
        {
            throw new UserFriendlyException("抽仓和订购类型不允许同时生成出库单!");
        }

        // 3. 生成出库单（委托工厂）
        LeaveStockBusinessTypeEnum businessType = LeaveStockBusinessTypeEnum.OrderLeaveStock;
        if (deductCount > 0)
        {
            businessType = LeaveStockBusinessTypeEnum.SalesLeaveStock;
        }
        ProductTypeEnum productType = batches[0].ProductType;
        BusinessOrderTypeEnum relationType = BusinessOrderTypeEnum.LeaveStockBatch;
        LeaveStockOrder model = await _leaveStockOrderFactory.CreateAsync(productType, null, relationType, businessType, null, cancellationToken);

        foreach (GenerateLeaveStockOrderFromBatchData data in datas)
        {
            // 3.1 查出库批次
            LeaveStockBatch? batch = batches.FirstOrDefault(x => x.Id == data.LeaveStockBatchId);
            if (batch == null)
            {
                throw new UserFriendlyException($"出库批次{data.LeaveStockBatchId}不存在!");
            }

            // 3.2 校验批次状态
            if (LeaveStockBatchStatusEnum.Pending != batch.Status)
            {
                throw new UserFriendlyException($"出库批次{batch.Number}当前状态不允许生成出库单!");
            }

            // 3.3 遍历批次项添加出库单明细
            foreach (UniqueSkuInfoData item in data.SkuInfos.Where(x => x.Qty != 0).ToList())
            {
                // 3.3.1 查出库批次项
                LeaveStockBatchItem? batchItem = batch.LeaveStockBatchItems.FirstOrDefault(x => x.Id == item.Id);
                if (batchItem == null)
                {
                    throw new UserFriendlyException("出库批次项不存在!");
                }

                // 3.3.2 查产品Sku、产品
                Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
                if (sku == null)
                {
                    throw new UserFriendlyException($"产品Sku{item.SkuId}不存在!");
                }
                Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
                if (spu == null)
                {
                    throw new UserFriendlyException($"产品{sku.SpuId}不存在!");
                }

                decimal price = spu.PurchasePrice;
                if (sku.PurchasePrice != null)
                {
                    price = sku.PurchasePrice.Value;
                }

                // 3.3.3 添加出库单明细
                model.AddItem(relationType, batch.Id, batchItem.Id, batch.Number, sku.SpuId, sku.Id, item.WarehouseId, item.Qty, price, batchItem.SkuAttributes, batchItem.OtherAttributes);
            }
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder> GenerateFromLeaveStockBatchNumberAsync(string batchNumber, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查出库批次
        LeaveStockBatch? batch = await _leaveStockBatchRepository.GetByNumberAsync(batchNumber, cancellationToken);
        if (batch == null)
        {
            throw new UserFriendlyException($"出库批次{batchNumber}不存在!");
        }
        if (LeaveStockBatchStatusEnum.Pending != batch.Status)
        {
            throw new UserFriendlyException($"出库批次{batchNumber}当前状态不允许生成出库单!");
        }

        // 2. 生成出库单（委托工厂）
        BusinessOrderTypeEnum relationType = BusinessOrderTypeEnum.LeaveStockBatch;
        LeaveStockBusinessTypeEnum businessType = LeaveStockBusinessTypeEnum.OrderLeaveStock;
        if (batch.LeaveStockPlanId != null)
        {
            businessType = LeaveStockBusinessTypeEnum.SalesLeaveStock;
        }
        LeaveStockOrder model = await _leaveStockOrderFactory.CreateAsync(batch.ProductType, null, relationType, businessType, null, cancellationToken);

        // 3. 添加明细
        foreach (LeaveStockBatchItem item in batch.LeaveStockBatchItems)
        {
            // 3.1 查产品Sku、产品
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku {item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品 {sku.SpuId}不存在!");
            }

            decimal price = spu.PurchasePrice;
            if (sku.PurchasePrice != null)
            {
                price = sku.PurchasePrice.Value;
            }

            // 3.2 添加出库单明细
            model.AddItem(relationType, batch.Id, item.Id, batch.Number, sku.SpuId, sku.Id, item.WarehouseId, item.Qty, price, item.SkuAttributes, item.OtherAttributes);
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder?> GenerateFromInventoryOrderAsync(long inventoryOrderId, long userId, string userName, CancellationToken cancellationToken = default)
    {
        InventoryOrder? inventoryOrder = await _inventoryOrderRepository.FindAsync(inventoryOrderId, true, cancellationToken);
        if (inventoryOrder == null)
        {
            throw new UserFriendlyException($"盘点单{inventoryOrderId}不存在!");
        }

        if (inventoryOrder.LossQty <= 0)
        {
            return null;
        }

        // 1. 生成出库单（委托工厂）
        BusinessOrderTypeEnum relationType = BusinessOrderTypeEnum.InventoryOrder;
        LeaveStockBusinessTypeEnum businessType = LeaveStockBusinessTypeEnum.InventoryLeaveStock;
        LeaveStockOrder model = await _leaveStockOrderFactory.CreateAsync(inventoryOrder.ProductType, null, relationType, businessType, inventoryOrder.Remark, cancellationToken);

        // 2. 添加明细
        foreach (InventoryOrderItem item in inventoryOrder.InventoryOrderItems.Where(x => x.LossQty > 0).ToList())
        {
            // 2.1 查产品Sku、产品
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku == null)
            {
                throw new UserFriendlyException($"产品Sku{item.SkuId}不存在!");
            }
            Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
            if (spu == null)
            {
                throw new UserFriendlyException($"产品{sku.SpuId}不存在!");
            }

            decimal price = spu.PurchasePrice;
            if (sku.PurchasePrice != null)
            {
                price = sku.PurchasePrice.Value;
            }

            // 2.2 添加出库单明细
            model.AddItem(relationType, inventoryOrder.Id, item.Id, inventoryOrder.Number, item.SpuId, item.SkuId, item.WarehouseId, item.LossQty, price, item.SkuAttributes, item.OtherAttributes);
        }

        model.Finish(userId, userName);
        return model;
    }

    /// <inheritdoc />
    public async Task<List<LeaveStockOrder>> GenerateFromRetailOrderBatchAsync(string retailOrderNumber, long retailOrderItemIdR, long spuIdR, long warehouseIdR, long retailOrderItemIdL, long spuIdL, long warehouseIdL, long userId, string userName, CancellationToken cancellationToken = default)
    {
        // 1. 查散镜订单
        RetailOrder? retailOrder = await _retailOrderRepository.GetByNumberAsync(retailOrderNumber, cancellationToken);
        if (retailOrder == null)
        {
            throw new UserFriendlyException($"散镜订单{retailOrderNumber}不存在!");
        }

        // 2. 查出库批次列表
        List<LeaveStockBatch> leaveStockBatches = await _leaveStockBatchRepository.GetListByRelationAsync(BusinessOrderTypeEnum.RetailOrder, retailOrder.Id, cancellationToken);
        if (leaveStockBatches.Count == 0)
        {
            throw new UserFriendlyException($"散镜订单{retailOrderNumber}出库批次不存在!");
        }

        // 3. 查散镜订单项
        RetailOrderItem? itemR = retailOrder.RetailOrderItems.FirstOrDefault(x => x.Id == retailOrderItemIdR);
        if (itemR == null)
        {
            throw new UserFriendlyException($"散镜订单项{retailOrderItemIdR}不存在!");
        }
        RetailOrderItem? itemL = retailOrder.RetailOrderItems.FirstOrDefault(x => x.Id == retailOrderItemIdL);
        if (itemL == null)
        {
            throw new UserFriendlyException($"散镜订单项{retailOrderItemIdL}不存在!");
        }

        // 4. 查产品
        Spu? spuR = await _spuRepository.FindAsync(spuIdR, true, cancellationToken);
        if (spuR == null)
        {
            throw new UserFriendlyException($"产品{spuIdR}不存在!");
        }
        Spu? spuL = await _spuRepository.FindAsync(spuIdL, true, cancellationToken);
        if (spuL == null)
        {
            throw new UserFriendlyException($"产品{spuIdL}不存在!");
        }

        List<LeaveStockOrder> list = new List<LeaveStockOrder>();

        // 5. 遍历出库批次生成出库单
        foreach (LeaveStockBatch leaveStockBatch in leaveStockBatches)
        {
            // 5.1 匹配散镜订单项对应的出库批次项
            LeaveStockBatchItem? leaveStockBatchItemR = leaveStockBatch.LeaveStockBatchItems.FirstOrDefault(x => x.RelationItemId == retailOrderItemIdR);
            LeaveStockBatchItem? leaveStockBatchItemL = leaveStockBatch.LeaveStockBatchItems.FirstOrDefault(x => x.RelationItemId == retailOrderItemIdL);

            // 5.2 构造出库单（委托工厂）
            LeaveStockOrder model = await _leaveStockOrderFactory.CreateAsync(leaveStockBatch.ProductType, null, BusinessOrderTypeEnum.LeaveStockBatch, LeaveStockBusinessTypeEnum.SalesLeaveStock, leaveStockBatch.Remark, cancellationToken);

            // 5.3 添加右眼明细
            if (leaveStockBatchItemR != null)
            {
                SortedDictionary<string, object?> skuAttributesR = new SortedDictionary<string, object?>();
                skuAttributesR["SPH"] = itemR.Sph;
                skuAttributesR["CYL"] = itemR.Cyl;
                string signatureR = Sku.CalculateSignature(spuIdR, skuAttributesR);
                Sku? skuR = await _skuRepository.GetByAttributesSignatureAsync(signatureR, cancellationToken);
                if (skuR == null)
                {
                    throw new UserFriendlyException($"产品{spuR.Code}产品Sku {SkuAttributeUtils.ToText(skuAttributesR)} 不存在!");
                }
                decimal priceR = spuR.PurchasePrice;
                if (skuR.PurchasePrice != null)
                {
                    priceR = skuR.PurchasePrice.Value;
                }
                model.AddItem(BusinessOrderTypeEnum.LeaveStockBatch, leaveStockBatch.Id, leaveStockBatchItemR.Id, leaveStockBatch.Number, spuIdR, skuR.Id, warehouseIdR, leaveStockBatchItemR.Qty, priceR, leaveStockBatchItemR.SkuAttributes, leaveStockBatchItemR.OtherAttributes);
            }

            // 5.4 添加左眼明细
            if (leaveStockBatchItemL != null)
            {
                SortedDictionary<string, object?> skuAttributesL = new SortedDictionary<string, object?>();
                skuAttributesL["SPH"] = itemL.Sph;
                skuAttributesL["CYL"] = itemL.Cyl;
                string signatureL = Sku.CalculateSignature(spuIdL, skuAttributesL);
                Sku? skuL = await _skuRepository.GetByAttributesSignatureAsync(signatureL, cancellationToken);
                if (skuL == null)
                {
                    throw new UserFriendlyException($"产品{spuL.Code}产品Sku {SkuAttributeUtils.ToText(skuAttributesL)} 不存在!");
                }
                decimal priceL = spuL.PurchasePrice;
                if (skuL.PurchasePrice != null)
                {
                    priceL = skuL.PurchasePrice.Value;
                }
                model.AddItem(BusinessOrderTypeEnum.LeaveStockBatch, leaveStockBatch.Id, leaveStockBatchItemL.Id, leaveStockBatch.Number, spuIdL, skuL.Id, warehouseIdL, leaveStockBatchItemL.Qty, priceL, leaveStockBatchItemL.SkuAttributes, leaveStockBatchItemL.OtherAttributes);
            }

            // 5.5 完成并收集出库单
            model.Finish(userId, userName);
            list.Add(model);
        }
        return list;
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder?> UpdateReversalQtyAsync(long reversalLeaveStockOrderId, CancellationToken cancellationToken = default)
    {
        // 1. 查红冲出库单
        LeaveStockOrder? reversalLeaveStockOrder = await _leaveStockOrderRepository.FindAsync(reversalLeaveStockOrderId, true, cancellationToken);
        if (reversalLeaveStockOrder == null)
        {
            throw new UserFriendlyException($"出库单{reversalLeaveStockOrderId}不存在!");
        }
        if (reversalLeaveStockOrder.OriginalId == null)
        {
            return null;
        }

        // 2. 查原出库单
        LeaveStockOrder? model = await _leaveStockOrderRepository.FindAsync(reversalLeaveStockOrder.OriginalId.Value, true, cancellationToken);
        if (model == null)
        {
            throw new UserFriendlyException("原出库单不存在!");
        }

        // 3. 累加冲销数量
        foreach (LeaveStockOrderItem item in reversalLeaveStockOrder.LeaveStockOrderItems)
        {
            if (item.OriginalId == null)
            {
                throw new UserFriendlyException($"出库单项{item.Id}不存在原出库单项Id!");
            }
            model.UpdateReversalQty(item.OriginalId.Value, Math.Abs(item.Qty));
        }
        return model;
    }

    /// <summary>
    /// Sku + 仓库组合，用于出库单项去重
    /// </summary>
    /// <param name="SkuId">产品SkuId</param>
    /// <param name="WarehouseId">仓库Id</param>
    private sealed record SkuWarehouse(long SkuId, long WarehouseId);
}
