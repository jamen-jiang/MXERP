using MXERP.Domain.BulkOrders;
using MXERP.Domain.Common.Contracts;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Common.Utils;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.PurchaseBatches;
using MXERP.Domain.PurchasePlans;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.Skus;
using MXERP.Domain.Stocks;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.BulkOrderItems;

/// <summary>
/// 批量订单项领域服务, 负责批量订单项的增删改和销售SKU行同步
/// </summary>
[ScopedService]
public class BulkOrderItemDomainService : IBulkOrderItemDomainService
{
    /// <summary>
    /// 批量订单仓储
    /// </summary>
    private readonly IBulkOrderRepository _bulkOrderRepository;
    /// <summary>
    /// 批量订单项仓储
    /// </summary>
    private readonly IBulkOrderItemRepository _bulkOrderItemRepository;
    /// <summary>
    /// 出库计划仓储
    /// </summary>
    private readonly ILeaveStockPlanRepository _leaveStockPlanRepository;
    /// <summary>
    /// 采购计划仓储
    /// </summary>
    private readonly IPurchasePlanRepository _purchasePlanRepository;
    /// <summary>
    /// 出库计划工厂
    /// </summary>
    private readonly ILeaveStockPlanFactory _leaveStockPlanFactory;
    /// <summary>
    /// 采购计划工厂
    /// </summary>
    private readonly IPurchasePlanFactory _purchasePlanFactory;
    /// <summary>
    /// 出库计划领域服务
    /// </summary>
    private readonly ILeaveStockPlanDomainService _leaveStockPlanDomainService;
    /// <summary>
    /// 采购计划领域服务
    /// </summary>
    private readonly IPurchasePlanDomainService _purchasePlanDomainService;
    /// <summary>
    /// 出库批次仓储
    /// </summary>
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;
    /// <summary>
    /// 采购批次仓储
    /// </summary>
    private readonly IPurchaseBatchRepository _purchaseBatchRepository;
    /// <summary>
    /// 销售产品仓储
    /// </summary>
    private readonly ISalesSpuRepository _salesSpuRepository;
    /// <summary>
    /// SKU仓储
    /// </summary>
    private readonly ISkuRepository _skuRepository;
    /// <summary>
    /// 库存仓储
    /// </summary>
    private readonly IStockRepository _stockRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="bulkOrderRepository">批量订单仓储</param>
    /// <param name="bulkOrderItemRepository">批量订单项仓储</param>
    /// <param name="leaveStockPlanRepository">出库计划仓储</param>
    /// <param name="purchasePlanRepository">采购计划仓储</param>
    /// <param name="leaveStockPlanFactory">出库计划工厂</param>
    /// <param name="purchasePlanFactory">采购计划工厂</param>
    /// <param name="leaveStockPlanDomainService">出库计划领域服务</param>
    /// <param name="purchasePlanDomainService">采购计划领域服务</param>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    /// <param name="purchaseBatchRepository">采购批次仓储</param>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="skuRepository">SKU仓储</param>
    /// <param name="stockRepository">库存仓储</param>
    public BulkOrderItemDomainService(IBulkOrderRepository bulkOrderRepository, IBulkOrderItemRepository bulkOrderItemRepository, ILeaveStockPlanRepository leaveStockPlanRepository, IPurchasePlanRepository purchasePlanRepository, ILeaveStockPlanFactory leaveStockPlanFactory, IPurchasePlanFactory purchasePlanFactory, ILeaveStockPlanDomainService leaveStockPlanDomainService, IPurchasePlanDomainService purchasePlanDomainService, ILeaveStockBatchRepository leaveStockBatchRepository, IPurchaseBatchRepository purchaseBatchRepository, ISalesSpuRepository salesSpuRepository, ISkuRepository skuRepository, IStockRepository stockRepository)
    {
        _bulkOrderRepository = bulkOrderRepository;
        _bulkOrderItemRepository = bulkOrderItemRepository;
        _leaveStockPlanRepository = leaveStockPlanRepository;
        _purchasePlanRepository = purchasePlanRepository;
        _leaveStockPlanFactory = leaveStockPlanFactory;
        _purchasePlanFactory = purchasePlanFactory;
        _leaveStockPlanDomainService = leaveStockPlanDomainService;
        _purchasePlanDomainService = purchasePlanDomainService;
        _leaveStockBatchRepository = leaveStockBatchRepository;
        _purchaseBatchRepository = purchaseBatchRepository;
        _salesSpuRepository = salesSpuRepository;
        _skuRepository = skuRepository;
        _stockRepository = stockRepository;
    }

    /// <inheritdoc />
    public async Task<BulkOrderItem> CreateAsync(long bulkOrderId, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, string? packetInfo, string? remark, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments, CancellationToken cancellationToken = default)
    {
        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(bulkOrderId, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单:" + bulkOrderId + "不存在!");
        }

        if (bulkOrder.Status != nameof(BulkOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("批量订单" + bulkOrder.Number + "当前状态不允许添加批量订单项!");
        }

        bool isExist = await _bulkOrderItemRepository.IsExistByBulkOrderIdAndSalesSpuIdExcludeIdAsync(bulkOrderId, salesSpuId, null, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("销售产品" + salesSpuId + " 已存在!");
        }

        return new BulkOrderItem(bulkOrderId, salesSpuId, salesSpuCode, customerSpuId, customerSpuCode, spuId, warehouseId, packetInfo, remark, attachmentIds, attachments);
    }

    /// <inheritdoc />
    public async Task<BulkOrderItem> ModifyAsync(long id, long? customerSpuId, string? customerSpuCode, string? packetInfo, string? remark, IReadOnlyCollection<long>? attachmentIds, IReadOnlyDictionary<string, byte[]>? attachments, CancellationToken cancellationToken = default)
    {
        BulkOrderItem? item = await _bulkOrderItemRepository.FindAsync(id, true, cancellationToken);
        if (item is null)
        {
            throw new UserFriendlyException("批量订单项" + id + "不存在!");
        }

        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(item.BulkOrderId, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单:" + item.BulkOrderId + "不存在!");
        }

        if (bulkOrder.Status != nameof(BulkOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("批量订单" + bulkOrder.Number + "当前状态不允许修改批量订单项!");
        }

        item.Modify(item.SalesSpuCode, customerSpuId, customerSpuCode, item.SpuId, item.WarehouseId, packetInfo, remark, attachmentIds, attachments);
        return item;
    }

    /// <inheritdoc />
    public async Task<BulkOrderItem> UnConfirmAsync(long id, CancellationToken cancellationToken = default)
    {
        BulkOrderItem? model = await _bulkOrderItemRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("批量订单项" + id + "不存在!");
        }

        bool hasLeaveStockBatch = await _leaveStockBatchRepository.IsExistByRelationAsync(nameof(BusinessOrderTypeEnum.BulkOrder), model.BulkOrderId, model.Id, cancellationToken);
        if (hasLeaveStockBatch)
        {
            throw new UserFriendlyException("批量订单项" + id + "已生成出库批次不允许取消确认!");
        }

        bool hasPurchaseBatch = await _purchaseBatchRepository.IsExistByRelationAsync(nameof(BusinessOrderTypeEnum.BulkOrder), model.BulkOrderId, model.Id, cancellationToken);
        if (hasPurchaseBatch)
        {
            throw new UserFriendlyException("批量订单项" + id + "已生成采购批次不允许取消确认!");
        }

        model.UnConfirm();
        return model;
    }

    /// <inheritdoc />
    public Task SyncSalesSkusAsync(BulkOrderItem item, IReadOnlyCollection<BulkOrderItemSalesSkuData>? salesSkus, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<BulkOrderItemSalesSkuData> target = salesSkus ?? new List<BulkOrderItemSalesSkuData>();
        HashSet<long> targetSkuIds = target.Select(x => x.SalesSkuId).ToHashSet();

        List<BulkOrderItemSalesSku> toRemoves = item.BulkOrderItemSalesSkus
            .Where(x => !targetSkuIds.Contains(x.SalesSkuId))
            .ToList();
        foreach (BulkOrderItemSalesSku salesSku in toRemoves)
        {
            item.DeleteLine(salesSku.Id);
        }

        Dictionary<long, BulkOrderItemSalesSku> existingMap = new Dictionary<long, BulkOrderItemSalesSku>();
        foreach (BulkOrderItemSalesSku salesSku in item.BulkOrderItemSalesSkus)
        {
            if (salesSku.IsDeleted)
            {
                continue;
            }

            existingMap[salesSku.SalesSkuId] = salesSku;
        }

        foreach (BulkOrderItemSalesSkuData data in target)
        {
            BulkOrderItemSalesSku? existing;
            if (existingMap.TryGetValue(data.SalesSkuId, out BulkOrderItemSalesSku? found))
            {
                existing = found;
            }
            else
            {
                existing = null;
            }

            if (existing is not null)
            {
                item.ModifyLine(existing.Id, data.OtherAttributes, data.Qty);
            }
            else
            {
                item.AddLine(data.SalesSkuId, data.SkuAttributes, data.OtherAttributes, data.Qty);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<BulkOrderItemAssignResult> AssignItemAsync(long id, IReadOnlyCollection<SkuInfoData> leaveStockPlanSkuInfos, IReadOnlyCollection<SkuInfoData> purchasePlanSkuInfos, CancellationToken cancellationToken = default)
    {
        BulkOrderItem? bulkOrderItem = await _bulkOrderItemRepository.FindAsync(id, true, cancellationToken);
        if (bulkOrderItem is null)
        {
            throw new UserFriendlyException("批量订单项:" + id + "不存在!");
        }

        if (bulkOrderItem.IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + id + "已确认不允许分配!");
        }

        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(bulkOrderItem.BulkOrderId, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单:" + bulkOrderItem.BulkOrderId + "不存在!");
        }

        string businessOrderType = nameof(BusinessOrderTypeEnum.BulkOrder);

        LeaveStockPlan? leaveStockPlan = await _leaveStockPlanRepository.GetByRelationAsync(businessOrderType, bulkOrder.Id, id, cancellationToken);
        if (leaveStockPlan is null)
        {
            leaveStockPlan = await _leaveStockPlanFactory.CreateAsync(bulkOrder.ProductType, bulkOrder.StoreId, bulkOrder.CompanyId, businessOrderType, bulkOrder.Id, bulkOrderItem.Id, bulkOrder.Number, bulkOrder.CustomerOrderNumber, bulkOrder.CustomerPONumber, bulkOrder.OrderRef, bulkOrderItem.Remark, cancellationToken);
        }

        PurchasePlan? purchasePlan = await _purchasePlanRepository.GetByRelationAsync(businessOrderType, bulkOrder.Id, id, cancellationToken);
        if (purchasePlan is null)
        {
            purchasePlan = await _purchasePlanFactory.CreateAsync(bulkOrder.ProductType, bulkOrder.StoreId, bulkOrder.CompanyId, businessOrderType, bulkOrder.Id, bulkOrderItem.Id, bulkOrder.Number, bulkOrder.CustomerOrderNumber, bulkOrder.CustomerPONumber, bulkOrder.OrderRef, bulkOrderItem.Remark, cancellationToken);
        }

        List<LeaveStockPlanItemData> leaveStockPlanItems = new List<LeaveStockPlanItemData>();
        double leaveStockQtySum = leaveStockPlanSkuInfos.Sum(x => x.Qty);
        if (leaveStockQtySum > 0)
        {
            foreach (SkuInfoData skuInfo in leaveStockPlanSkuInfos)
            {
                if (skuInfo.Qty <= 0)
                {
                    continue;
                }

                Sku? sku = await _skuRepository.FindAsync(skuInfo.SkuId, true, cancellationToken);
                if (sku is null)
                {
                    throw new UserFriendlyException("产品Sku " + skuInfo.SkuId + "不存在!");
                }

                BulkOrderItemSalesSku? salesSku = bulkOrderItem.BulkOrderItemSalesSkus
                    .FirstOrDefault(x => AreEqual(sku.KeyAttributes, x.SkuAttributes));
                if (salesSku is null)
                {
                    throw new UserFriendlyException("批量订单:" + bulkOrder.Number + "销售产品:" + bulkOrderItem.SalesSpuCode + "产品Sku:" + SkuAttributeUtils.ToText(sku.KeyAttributes) + "不存在!");
                }

                double otherQty = leaveStockPlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);
                if (salesSku.Qty - otherQty < skuInfo.Qty)
                {
                    throw new UserFriendlyException("批量订单:" + bulkOrder.Number + "销售产品:" + bulkOrderItem.SalesSpuCode + "产品Sku:" + SkuAttributeUtils.ToText(sku.KeyAttributes) + "超出可生成数量!");
                }

                leaveStockPlanItems.Add(new LeaveStockPlanItemData(salesSku.Id, bulkOrderItem.SalesSpuId, bulkOrderItem.SalesSpuCode, bulkOrderItem.CustomerSpuId, bulkOrderItem.CustomerSpuCode, skuInfo.SkuId, skuInfo.WarehouseId, skuInfo.Qty, salesSku.SkuAttributes, salesSku.OtherAttributes));
            }
        }

        List<PurchasePlanItemData> purchasePlanItems = new List<PurchasePlanItemData>();
        double purchaseQtySum = purchasePlanSkuInfos.Sum(x => x.Qty);
        if (purchaseQtySum > 0)
        {
            foreach (SkuInfoData skuInfo in purchasePlanSkuInfos)
            {
                if (skuInfo.Qty <= 0)
                {
                    continue;
                }

                Sku? sku = await _skuRepository.FindAsync(skuInfo.SkuId, true, cancellationToken);
                if (sku is null)
                {
                    throw new UserFriendlyException("产品Sku " + skuInfo.SkuId + "不存在!");
                }

                BulkOrderItemSalesSku? salesSku = bulkOrderItem.BulkOrderItemSalesSkus
                    .FirstOrDefault(x => AreEqual(sku.KeyAttributes, x.SkuAttributes));
                if (salesSku is null)
                {
                    throw new UserFriendlyException("批量订单:" + bulkOrder.Number + "销售产品:" + bulkOrderItem.SalesSpuCode + "产品Sku:" + SkuAttributeUtils.ToText(sku.KeyAttributes) + "不存在!");
                }

                double otherQty = purchasePlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);
                otherQty = otherQty + leaveStockPlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);
                if (salesSku.Qty - otherQty < skuInfo.Qty)
                {
                    throw new UserFriendlyException("批量订单:" + bulkOrder.Number + "销售产品:" + bulkOrderItem.SalesSpuCode + ":产品Sku:" + SkuAttributeUtils.ToText(sku.KeyAttributes) + "超出可生成数量!");
                }

                purchasePlanItems.Add(new PurchasePlanItemData(salesSku.Id, bulkOrderItem.SalesSpuId, bulkOrderItem.SalesSpuCode, bulkOrderItem.CustomerSpuId, bulkOrderItem.CustomerSpuCode, skuInfo.SkuId, skuInfo.WarehouseId, skuInfo.Qty, salesSku.SkuAttributes, salesSku.OtherAttributes));
            }
        }

        await _leaveStockPlanDomainService.SyncLeaveStockPlanItemsAsync(leaveStockPlan, leaveStockPlanItems, cancellationToken);
        await _purchasePlanDomainService.SyncPurchasePlanItemsAsync(purchasePlan, purchasePlanItems, cancellationToken);

        return new BulkOrderItemAssignResult(leaveStockPlan, purchasePlan);
    }

    /// <inheritdoc />
    public async Task<BulkOrderItemAssignResult> AutomaticAssignItemAsync(long id, CancellationToken cancellationToken = default)
    {
        BulkOrderItem? bulkOrderItem = await _bulkOrderItemRepository.FindAsync(id, true, cancellationToken);
        if (bulkOrderItem is null)
        {
            throw new UserFriendlyException("批量订单项:" + id + "不存在!");
        }

        if (bulkOrderItem.IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + id + "已确认不允许分配!");
        }

        return await AutomaticAssignItemAsync(bulkOrderItem, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BulkOrderItemAssignResult> AutomaticAssignItemAsync(BulkOrderItem bulkOrderItem, CancellationToken cancellationToken = default)
    {
        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(bulkOrderItem.BulkOrderId, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单:" + bulkOrderItem.BulkOrderId + "不存在!");
        }

        if (bulkOrderItem.IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + bulkOrderItem.Id + "已确认不允许分配!");
        }

        return await DoAutomaticAssignAsync(bulkOrder, bulkOrderItem, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BulkOrderItemAssignResult> AllCustomizedAssignItemAsync(long id, CancellationToken cancellationToken = default)
    {
        BulkOrderItem? bulkOrderItem = await _bulkOrderItemRepository.FindAsync(id, true, cancellationToken);
        if (bulkOrderItem is null)
        {
            throw new UserFriendlyException("批量订单项:" + id + "不存在!");
        }

        if (bulkOrderItem.IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + id + "已确认不允许分配!");
        }

        return await AllCustomizedAssignItemAsync(bulkOrderItem, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BulkOrderItemAssignResult> AllCustomizedAssignItemAsync(BulkOrderItem bulkOrderItem, CancellationToken cancellationToken = default)
    {
        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(bulkOrderItem.BulkOrderId, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单:" + bulkOrderItem.BulkOrderId + "不存在!");
        }

        if (bulkOrderItem.IsConfirm)
        {
            throw new UserFriendlyException("批量订单项:" + bulkOrderItem.Id + "已确认不允许分配!");
        }

        return await DoAllCustomizedAssignAsync(bulkOrder, bulkOrderItem, cancellationToken);
    }

    /// <summary>
    /// 自动分配单项
    /// </summary>
    /// <param name="bulkOrder">批量订单</param>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    private async Task<BulkOrderItemAssignResult> DoAutomaticAssignAsync(BulkOrder bulkOrder, BulkOrderItem bulkOrderItem, CancellationToken cancellationToken = default)
    {
        SalesSpu? salesSpu = await _salesSpuRepository.FindAsync(bulkOrderItem.SalesSpuId, true, cancellationToken);
        if (salesSpu is null)
        {
            throw new UserFriendlyException("批量订单" + bulkOrder.Number + ":销售产品不存在!");
        }

        string businessOrderType = nameof(BusinessOrderTypeEnum.BulkOrder);

        LeaveStockPlan? leaveStockPlan = await _leaveStockPlanRepository.GetByRelationAsync(businessOrderType, bulkOrder.Id, bulkOrderItem.Id, cancellationToken);
        if (leaveStockPlan is null)
        {
            leaveStockPlan = await _leaveStockPlanFactory.CreateAsync(bulkOrder.ProductType, bulkOrder.StoreId, bulkOrder.CompanyId, businessOrderType, bulkOrder.Id, bulkOrderItem.Id, bulkOrder.Number, bulkOrder.CustomerOrderNumber, bulkOrder.CustomerPONumber, bulkOrder.OrderRef, bulkOrderItem.Remark, cancellationToken);
        }

        PurchasePlan? purchasePlan = await _purchasePlanRepository.GetByRelationAsync(businessOrderType, bulkOrder.Id, bulkOrderItem.Id, cancellationToken);
        if (purchasePlan is null)
        {
            purchasePlan = await _purchasePlanFactory.CreateAsync(bulkOrder.ProductType, bulkOrder.StoreId, bulkOrder.CompanyId, businessOrderType, bulkOrder.Id, bulkOrderItem.Id, bulkOrder.Number, bulkOrder.CustomerOrderNumber, bulkOrder.CustomerPONumber, bulkOrder.OrderRef, bulkOrderItem.Remark, cancellationToken);
        }

        List<LeaveStockPlanItemData> leaveStockPlanItems = new List<LeaveStockPlanItemData>();
        List<PurchasePlanItemData> purchasePlanItems = new List<PurchasePlanItemData>();

        List<SalesSpuAssignItem> sortedBySequence = salesSpu.SalesSpuAssignItems
            .OrderBy(x => x.Sequence)
            .ToList();

        foreach (SalesSpuAssignItem assignItem in sortedBySequence)
        {
            foreach (BulkOrderItemSalesSku salesSku in bulkOrderItem.BulkOrderItemSalesSkus)
            {
                string signature = Sku.CalculateSignature(assignItem.SpuId, salesSku.SkuAttributes);
                Sku? sku = await _skuRepository.GetByAttributesSignatureAsync(signature, cancellationToken);
                if (sku is null)
                {
                    continue;
                }

                Stock? stock = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(sku.Id, assignItem.WarehouseId, cancellationToken);

                double otherQty = leaveStockPlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);

                double alreadyQty = leaveStockPlan.LeaveStockPlanItems
                    .Where(x => x.SkuId == sku.Id && x.WarehouseId == assignItem.WarehouseId)
                    .Sum(x => x.Qty);

                double noAssignQty = salesSku.Qty - otherQty;
                if (stock is null || noAssignQty <= 0)
                {
                    continue;
                }

                double actualQty = stock.GetActualQty() + alreadyQty;
                double remainQty = actualQty - noAssignQty;
                if (remainQty >= 0)
                {
                    leaveStockPlanItems.Add(GetLeaveStockPlanItem(bulkOrderItem, sku, assignItem.WarehouseId, noAssignQty));
                }
                else if (actualQty > 0)
                {
                    leaveStockPlanItems.Add(GetLeaveStockPlanItem(bulkOrderItem, sku, assignItem.WarehouseId, actualQty));
                }
            }
        }

        List<SalesSpuAssignItem> sortedByPriority = salesSpu.SalesSpuAssignItems
            .OrderByDescending(x => x.IsPriorityCustomized)
            .ThenBy(x => x.Sequence)
            .ToList();

        foreach (SalesSpuAssignItem assignItem in sortedByPriority)
        {
            foreach (BulkOrderItemSalesSku salesSku in bulkOrderItem.BulkOrderItemSalesSkus)
            {
                string signature = Sku.CalculateSignature(assignItem.SpuId, salesSku.SkuAttributes);
                Sku? sku = await _skuRepository.GetByAttributesSignatureAsync(signature, cancellationToken);
                if (sku is null)
                {
                    continue;
                }

                double otherQty = leaveStockPlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);
                otherQty = otherQty + purchasePlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);

                double noAssignQty = salesSku.Qty - otherQty;
                if (noAssignQty > 0)
                {
                    purchasePlanItems.Add(GetPurchasePlanItem(bulkOrderItem, sku, assignItem.WarehouseId, noAssignQty));
                }
            }
        }

        await _leaveStockPlanDomainService.SyncLeaveStockPlanItemsAsync(leaveStockPlan, leaveStockPlanItems, cancellationToken);
        await _purchasePlanDomainService.SyncPurchasePlanItemsAsync(purchasePlan, purchasePlanItems, cancellationToken);

        return new BulkOrderItemAssignResult(leaveStockPlan, purchasePlan);
    }

    /// <summary>
    /// 全订购分配单项
    /// </summary>
    /// <param name="bulkOrder">批量订单</param>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>分配结果</returns>
    private async Task<BulkOrderItemAssignResult> DoAllCustomizedAssignAsync(BulkOrder bulkOrder, BulkOrderItem bulkOrderItem, CancellationToken cancellationToken = default)
    {
        SalesSpu? salesSpu = await _salesSpuRepository.FindAsync(bulkOrderItem.SalesSpuId, true, cancellationToken);
        if (salesSpu is null)
        {
            throw new UserFriendlyException("销售产品:" + bulkOrderItem.SalesSpuId + "不存在!");
        }

        string businessOrderType = nameof(BusinessOrderTypeEnum.BulkOrder);

        LeaveStockPlan? leaveStockPlan = await _leaveStockPlanRepository.GetByRelationAsync(businessOrderType, bulkOrderItem.BulkOrderId, bulkOrderItem.Id, cancellationToken);
        if (leaveStockPlan is null)
        {
            leaveStockPlan = await _leaveStockPlanFactory.CreateAsync(bulkOrder.ProductType, bulkOrder.StoreId, bulkOrder.CompanyId, businessOrderType, bulkOrder.Id, bulkOrderItem.Id, bulkOrder.Number, bulkOrder.CustomerOrderNumber, bulkOrder.CustomerPONumber, bulkOrder.OrderRef, bulkOrderItem.Remark, cancellationToken);
        }

        PurchasePlan? purchasePlan = await _purchasePlanRepository.GetByRelationAsync(businessOrderType, bulkOrder.Id, bulkOrderItem.Id, cancellationToken);
        if (purchasePlan is null)
        {
            purchasePlan = await _purchasePlanFactory.CreateAsync(bulkOrder.ProductType, bulkOrder.StoreId, bulkOrder.CompanyId, businessOrderType, bulkOrder.Id, bulkOrderItem.Id, bulkOrder.Number, bulkOrder.CustomerOrderNumber, bulkOrder.CustomerPONumber, bulkOrder.OrderRef, bulkOrderItem.Remark, cancellationToken);
        }

        List<PurchasePlanItemData> purchasePlanItems = new List<PurchasePlanItemData>();

        List<SalesSpuAssignItem> sortedAssignItems = salesSpu.SalesSpuAssignItems
            .OrderByDescending(x => x.IsPriorityCustomized)
            .ThenBy(x => x.Sequence)
            .ToList();

        foreach (SalesSpuAssignItem assignItem in sortedAssignItems)
        {
            foreach (BulkOrderItemSalesSku salesSku in bulkOrderItem.BulkOrderItemSalesSkus)
            {
                string signature = Sku.CalculateSignature(assignItem.SpuId, salesSku.SkuAttributes);
                Sku? sku = await _skuRepository.GetByAttributesSignatureAsync(signature, cancellationToken);
                if (sku is null)
                {
                    continue;
                }

                double otherQty = purchasePlanItems
                    .Where(x => AreEqual(sku.KeyAttributes, x.SkuAttributes))
                    .Sum(x => x.Qty);
                double noAssignQty = salesSku.Qty - otherQty;
                if (noAssignQty > 0)
                {
                    purchasePlanItems.Add(GetPurchasePlanItem(bulkOrderItem, sku, assignItem.WarehouseId, noAssignQty));
                }
            }
        }

        await _leaveStockPlanDomainService.SyncLeaveStockPlanItemsAsync(leaveStockPlan, null, cancellationToken);
        await _purchasePlanDomainService.SyncPurchasePlanItemsAsync(purchasePlan, purchasePlanItems, cancellationToken);

        return new BulkOrderItemAssignResult(leaveStockPlan, purchasePlan);
    }

    /// <summary>
    /// 构造出库计划项
    /// </summary>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="sku">产品Sku</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <returns>出库计划项数据</returns>
    private static LeaveStockPlanItemData GetLeaveStockPlanItem(BulkOrderItem bulkOrderItem, Sku sku, long warehouseId, double qty)
    {
        BulkOrderItemSalesSku? salesSku = bulkOrderItem.BulkOrderItemSalesSkus
            .FirstOrDefault(x => AreEqual(sku.KeyAttributes, x.SkuAttributes));
        if (salesSku is null || salesSku.Qty == 0)
        {
            throw new UserFriendlyException("产品Sku:" + SkuAttributeUtils.ToText(sku.KeyAttributes) + "出库不允许超出下单范围!");
        }

        return new LeaveStockPlanItemData(salesSku.Id, bulkOrderItem.SalesSpuId, bulkOrderItem.SalesSpuCode, bulkOrderItem.CustomerSpuId, bulkOrderItem.CustomerSpuCode, sku.Id, warehouseId, qty, sku.KeyAttributes, salesSku.OtherAttributes);
    }

    /// <summary>
    /// 构造采购计划项
    /// </summary>
    /// <param name="bulkOrderItem">批量订单项</param>
    /// <param name="sku">产品Sku</param>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="qty">数量</param>
    /// <returns>采购计划项数据</returns>
    private static PurchasePlanItemData GetPurchasePlanItem(BulkOrderItem bulkOrderItem, Sku sku, long warehouseId, double qty)
    {
        BulkOrderItemSalesSku? salesSku = bulkOrderItem.BulkOrderItemSalesSkus
            .FirstOrDefault(x => AreEqual(sku.KeyAttributes, x.SkuAttributes));
        if (salesSku is null || salesSku.Qty == 0)
        {
            throw new UserFriendlyException("产品Sku:" + SkuAttributeUtils.ToText(sku.KeyAttributes) + "订购不允许超出下单范围!");
        }

        return new PurchasePlanItemData(salesSku.Id, bulkOrderItem.SalesSpuId, bulkOrderItem.SalesSpuCode, bulkOrderItem.CustomerSpuId, bulkOrderItem.CustomerSpuCode, sku.Id, warehouseId, qty, sku.KeyAttributes, salesSku.OtherAttributes);
    }

    /// <summary>
    /// 比较两个属性字典是否相等
    /// </summary>
    /// <param name="left">左侧属性字典(可空)</param>
    /// <param name="right">右侧属性字典(可空)</param>
    /// <returns>相等返回 true</returns>
    private static bool AreEqual(SortedDictionary<string, object?>? left, SortedDictionary<string, object?>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        if (left.Count != right.Count)
        {
            return false;
        }

        foreach (KeyValuePair<string, object?> pair in left)
        {
            object? rightValue;
            if (!right.TryGetValue(pair.Key, out rightValue))
            {
                return false;
            }

            string leftText = pair.Value?.ToString() ?? "null";
            string rightText = rightValue?.ToString() ?? "null";
            if (!string.Equals(leftText, rightText, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
