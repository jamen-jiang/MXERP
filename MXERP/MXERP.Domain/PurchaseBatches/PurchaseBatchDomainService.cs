using MXERP.Domain.BulkOrderItems;
using MXERP.Domain.BulkOrders;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.PurchasePlans;
using MXERP.Domain.PurchaseRequisitionOrders;
using MXERP.Domain.Skus;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseBatches;

/// <summary>
/// 采购批次领域服务, 负责按业务单据生成采购批次与弃审
/// </summary>
[ScopedService]
public class PurchaseBatchDomainService : IPurchaseBatchDomainService
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
    /// 出库批次仓储
    /// </summary>
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;
    /// <summary>
    /// 采购批次仓储
    /// </summary>
    private readonly IPurchaseBatchRepository _purchaseBatchRepository;
    /// <summary>
    /// 产品Sku仓储
    /// </summary>
    private readonly ISkuRepository _skuRepository;
    /// <summary>
    /// 采购计划仓储
    /// </summary>
    private readonly IPurchasePlanRepository _purchasePlanRepository;
    /// <summary>
    /// 申购单仓储
    /// </summary>
    private readonly IPurchaseRequisitionOrderRepository _purchaseRequisitionOrderRepository;
    /// <summary>
    /// 采购批次工厂
    /// </summary>
    private readonly IPurchaseBatchFactory _purchaseBatchFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="bulkOrderRepository">批量订单仓储</param>
    /// <param name="bulkOrderItemRepository">批量订单项仓储</param>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    /// <param name="purchaseBatchRepository">采购批次仓储</param>
    /// <param name="skuRepository">产品Sku仓储</param>
    /// <param name="purchasePlanRepository">采购计划仓储</param>
    /// <param name="purchaseRequisitionOrderRepository">申购单仓储</param>
    /// <param name="purchaseBatchFactory">采购批次工厂</param>
    public PurchaseBatchDomainService(IBulkOrderRepository bulkOrderRepository, IBulkOrderItemRepository bulkOrderItemRepository, ILeaveStockBatchRepository leaveStockBatchRepository, IPurchaseBatchRepository purchaseBatchRepository, ISkuRepository skuRepository, IPurchasePlanRepository purchasePlanRepository, IPurchaseRequisitionOrderRepository purchaseRequisitionOrderRepository, IPurchaseBatchFactory purchaseBatchFactory)
    {
        _bulkOrderRepository = bulkOrderRepository;
        _bulkOrderItemRepository = bulkOrderItemRepository;
        _leaveStockBatchRepository = leaveStockBatchRepository;
        _purchaseBatchRepository = purchaseBatchRepository;
        _skuRepository = skuRepository;
        _purchasePlanRepository = purchasePlanRepository;
        _purchaseRequisitionOrderRepository = purchaseRequisitionOrderRepository;
        _purchaseBatchFactory = purchaseBatchFactory;
    }

    /// <inheritdoc />
    public async Task<PurchaseBatch?> GenerateFromCheckPurchaseRequisitionOrderAsync(long purchaseRequisitionOrderId, CancellationToken cancellationToken = default)
    {
        PurchaseRequisitionOrder? purchaseRequisitionOrder = await _purchaseRequisitionOrderRepository.FindAsync(purchaseRequisitionOrderId, true, cancellationToken);
        if (purchaseRequisitionOrder is null)
        {
            throw new UserFriendlyException("申购单" + purchaseRequisitionOrderId + "不存在!");
        }

        PurchasePlan? plan = await _purchasePlanRepository.GetByRelationAsync(nameof(BusinessOrderTypeEnum.PurchaseRequisitionOrder), purchaseRequisitionOrderId, null, cancellationToken);
        if (plan is null || plan.PurchasePlanItems.Count == 0)
        {
            return null;
        }

        PurchaseBatch model = await _purchaseBatchFactory.CreateAsync(plan.ProductType, plan.Id, plan.DepartmentId ?? 0, plan.CompanyId, plan.RelationType, plan.RelationId, plan.RelationNumber, plan.Remark, cancellationToken);

        foreach (PurchasePlanItem item in plan.PurchasePlanItems)
        {
            model.AddItem(item.RelationItemId, item.SpuId, item.SkuId, item.WarehouseId, item.Qty, item.SkuAttributes, item.OtherAttributes);
        }

        return model;
    }

    /// <inheritdoc />
    public async Task<List<PurchaseBatch>> GenerateFromBulkOrderItemAsync(long bulkOrderItemId, CancellationToken cancellationToken = default)
    {
        BulkOrderItem? bulkOrderItem = await _bulkOrderItemRepository.FindAsync(bulkOrderItemId, true, cancellationToken);
        if (bulkOrderItem is null)
        {
            throw new UserFriendlyException("批量订单项" + bulkOrderItemId + "不存在!");
        }

        BulkOrder? bulkOrder = await _bulkOrderRepository.FindAsync(bulkOrderItem.BulkOrderId, true, cancellationToken);
        if (bulkOrder is null)
        {
            throw new UserFriendlyException("批量订单" + bulkOrderItem.BulkOrderId + "不存在!");
        }

        if (bulkOrder.Status == nameof(BulkOrderStatusEnum.Canceled))
        {
            throw new UserFriendlyException("批量订单" + bulkOrder.Number + "已取消不允许生成采购批次!");
        }

        bool isExist = await _purchaseBatchRepository.IsExistByRelationAsync(nameof(BusinessOrderTypeEnum.BulkOrder), bulkOrderItem.BulkOrderId, bulkOrderItem.Id, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("批量订单项" + bulkOrderItemId + "已生成采购批次!");
        }

        List<PurchaseBatch> list = new List<PurchaseBatch>();

        PurchasePlan? plan = await _purchasePlanRepository.GetByRelationAsync(nameof(BusinessOrderTypeEnum.BulkOrder), bulkOrderItem.BulkOrderId, bulkOrderItem.Id, cancellationToken);
        if (plan is null || plan.PurchasePlanItems.Count == 0)
        {
            return list;
        }

        List<SpuWarehouse> spuWarehouses = plan.PurchasePlanItems
            .Select(x => new SpuWarehouse(x.SpuId, x.WarehouseId))
            .Distinct()
            .ToList();

        foreach (SpuWarehouse spuWarehouse in spuWarehouses)
        {
            PurchaseBatch model = await _purchaseBatchFactory.CreateAsync(plan.ProductType, plan.Id, plan.StoreId ?? 0, plan.CompanyId, plan.RelationType, plan.RelationId, plan.RelationItemId, plan.RelationNumber, plan.RelationCustomerOrderNumber, plan.RelationCustomerPONumber, plan.RelationOrderRef, plan.Remark, cancellationToken);

            List<PurchasePlanItem> purchasePlanItems = plan.PurchasePlanItems
                .Where(x => x.SpuId == spuWarehouse.SpuId && x.WarehouseId == spuWarehouse.WarehouseId)
                .ToList();

            foreach (PurchasePlanItem item in purchasePlanItems)
            {
                model.AddItem(item.RelationItemId, item.SalesSpuId ?? 0, item.SalesSpuCode ?? string.Empty, item.CustomerSpuId, item.CustomerSpuCode, item.SpuId, item.SkuId, item.WarehouseId, item.Qty, item.SkuAttributes, item.OtherAttributes);
            }

            list.Add(model);
        }

        return list;
    }

    /// <inheritdoc />
    public async Task<List<PurchaseBatch>> GenerateFromLeaveStockBatchAsync(long leaveStockBatchId, CancellationToken cancellationToken = default)
    {
        List<PurchaseBatch> list = new List<PurchaseBatch>();

        LeaveStockBatch? leaveStockBatch = await _leaveStockBatchRepository.FindAsync(leaveStockBatchId, true, cancellationToken);
        if (leaveStockBatch is null)
        {
            throw new UserFriendlyException("出库批次不存在!");
        }

        if (leaveStockBatch.PurchaseBatchId is not null)
        {
            return list;
        }

        List<PurchaseBatch> alreadyGeneratePurchaseBatches = await _purchaseBatchRepository.GetListByRelationAsync(nameof(BusinessOrderTypeEnum.LeaveStockBatch), leaveStockBatchId, cancellationToken);

        if (leaveStockBatch.ProductType == nameof(ProductTypeEnum.Accessory))
        {
            PurchaseBatch model = await _purchaseBatchFactory.CreateAsync(nameof(ProductTypeEnum.Accessory), null, leaveStockBatch.DepartmentId ?? 0, leaveStockBatch.CompanyId, nameof(BusinessOrderTypeEnum.LeaveStockBatch), leaveStockBatchId, leaveStockBatch.Number, leaveStockBatch.Remark, cancellationToken);

            foreach (LeaveStockBatchItem item in leaveStockBatch.LeaveStockBatchItems)
            {
                Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
                if (sku is null)
                {
                    throw new UserFriendlyException("产品Sku " + item.SkuId + "不存在!");
                }

                double alreadyQty = alreadyGeneratePurchaseBatches
                    .SelectMany(x => x.PurchaseBatchItems)
                    .Where(x => x.SkuId == item.SkuId && x.WarehouseId == item.WarehouseId && x.RelationItemId == item.Id)
                    .Sum(x => x.Qty);

                double qty = item.Qty - item.LeaveStockQty - alreadyQty;
                if (qty < 0)
                {
                    throw new UserFriendlyException("出库批次" + leaveStockBatch.Number + "已超出可采购数量,请取消对应采购批次!");
                }

                model.AddItem(item.Id, sku.SpuId, item.SkuId, item.WarehouseId, qty, sku.KeyAttributes, item.OtherAttributes);
            }

            if (model.Qty > 0)
            {
                list.Add(model);
            }

            return list;
        }

        List<SpuWarehouse> spuWarehouses = leaveStockBatch.LeaveStockBatchItems
            .Select(x => new SpuWarehouse(x.SpuId, x.WarehouseId))
            .Distinct()
            .ToList();

        foreach (SpuWarehouse spuWarehouse in spuWarehouses)
        {
            PurchaseBatch model = await _purchaseBatchFactory.CreateAsync(leaveStockBatch.ProductType, null, leaveStockBatch.StoreId ?? 0, leaveStockBatch.CompanyId, nameof(BusinessOrderTypeEnum.LeaveStockBatch), leaveStockBatchId, null, leaveStockBatch.Number, leaveStockBatch.RelationCustomerOrderNumber, leaveStockBatch.RelationCustomerPONumber, leaveStockBatch.RelationOrderRef, leaveStockBatch.Remark, cancellationToken);

            List<LeaveStockBatchItem> leaveStockBatchItems = leaveStockBatch.LeaveStockBatchItems
                .Where(x => x.SpuId == spuWarehouse.SpuId && x.WarehouseId == spuWarehouse.WarehouseId)
                .ToList();

            foreach (LeaveStockBatchItem item in leaveStockBatchItems)
            {
                Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
                if (sku is null)
                {
                    throw new UserFriendlyException("产品Sku " + item.SkuId + "不存在!");
                }

                double alreadyQty = alreadyGeneratePurchaseBatches
                    .SelectMany(x => x.PurchaseBatchItems)
                    .Where(x => x.SkuId == item.SkuId && x.WarehouseId == item.WarehouseId && x.RelationItemId == item.Id)
                    .Sum(x => x.Qty);

                double qty = item.Qty - item.LeaveStockQty - alreadyQty;
                if (qty < 0)
                {
                    throw new UserFriendlyException("出库批次" + leaveStockBatch.Number + "已超出可采购数量,请取消对应采购批次!");
                }

                model.AddItem(item.Id, item.SalesSpuId ?? 0, item.SalesSpuCode ?? string.Empty, item.CustomerSpuId, item.CustomerSpuCode, sku.SpuId, item.SkuId, item.WarehouseId, qty, sku.KeyAttributes, item.OtherAttributes);
            }

            if (model.Qty > 0)
            {
                list.Add(model);
            }
        }

        return list;
    }

    /// <inheritdoc />
    public async Task<PurchaseBatch> UnCheckAsync(long id, string? remark, CancellationToken cancellationToken = default)
    {
        PurchaseBatch? model = await _purchaseBatchRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("采购批次不存在!");
        }

        bool hasLeaveStockBatch = await _leaveStockBatchRepository.IsExistByPurchaseBatchIdAsync(model.Id, cancellationToken);
        if (hasLeaveStockBatch)
        {
            throw new UserFriendlyException("采购批次" + model.Number + "已生成出库批次,不允许弃审!");
        }

        model.UnCheck(remark);
        return model;
    }

    /// <summary>
    /// SPU + 仓库组合(用于去重)
    /// </summary>
    /// <param name="SpuId">产品Id</param>
    /// <param name="WarehouseId">仓库Id</param>
    private sealed record SpuWarehouse(long SpuId, long WarehouseId);
}
