using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.Common.Contracts;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.Departments;
using MXERP.Domain.LeaveStockPlans;
using MXERP.Domain.PurchasePlans;
using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using MXERP.Domain.Stocks;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单领域服务
/// </summary>
[ScopedService]
public class PurchaseRequisitionOrderDomainService : IPurchaseRequisitionOrderDomainService
{
    /// <summary>
    /// 自动分配默认仓库Id
    /// </summary>
    private const long DefaultWarehouseId = 15;

    private readonly IPurchaseRequisitionOrderRepository _purchaseRequisitionOrderRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILeaveStockPlanRepository _leaveStockPlanRepository;
    private readonly IPurchasePlanRepository _purchasePlanRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ILeaveStockPlanDomainService _leaveStockPlanDomainService;
    private readonly IPurchasePlanDomainService _purchasePlanDomainService;
    private readonly ILeaveStockPlanFactory _leaveStockPlanFactory;
    private readonly IPurchasePlanFactory _purchasePlanFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseRequisitionOrderRepository">申购单仓储</param>
    /// <param name="departmentRepository">部门仓储</param>
    /// <param name="leaveStockPlanRepository">出库计划仓储</param>
    /// <param name="purchasePlanRepository">采购计划仓储</param>
    /// <param name="skuRepository">产品Sku仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="stockRepository">库存仓储</param>
    /// <param name="leaveStockPlanDomainService">出库计划领域服务</param>
    /// <param name="purchasePlanDomainService">采购计划领域服务</param>
    /// <param name="leaveStockPlanFactory">出库计划工厂</param>
    /// <param name="purchasePlanFactory">采购计划工厂</param>
    public PurchaseRequisitionOrderDomainService(
        IPurchaseRequisitionOrderRepository purchaseRequisitionOrderRepository,
        IDepartmentRepository departmentRepository,
        ILeaveStockPlanRepository leaveStockPlanRepository,
        IPurchasePlanRepository purchasePlanRepository,
        ISkuRepository skuRepository,
        ISpuRepository spuRepository,
        IStockRepository stockRepository,
        ILeaveStockPlanDomainService leaveStockPlanDomainService,
        IPurchasePlanDomainService purchasePlanDomainService,
        ILeaveStockPlanFactory leaveStockPlanFactory,
        IPurchasePlanFactory purchasePlanFactory)
    {
        _purchaseRequisitionOrderRepository = purchaseRequisitionOrderRepository;
        _departmentRepository = departmentRepository;
        _leaveStockPlanRepository = leaveStockPlanRepository;
        _purchasePlanRepository = purchasePlanRepository;
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _stockRepository = stockRepository;
        _leaveStockPlanDomainService = leaveStockPlanDomainService;
        _purchasePlanDomainService = purchasePlanDomainService;
        _leaveStockPlanFactory = leaveStockPlanFactory;
        _purchasePlanFactory = purchasePlanFactory;
    }

    /// <inheritdoc />
    public async Task SyncPurchaseRequisitionOrderItemsAsync(PurchaseRequisitionOrder purchaseRequisitionOrder, IReadOnlyCollection<PurchaseRequisitionOrderItemData>? purchaseRequisitionOrderItems, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<PurchaseRequisitionOrderItemData> finalItems = purchaseRequisitionOrderItems ?? new List<PurchaseRequisitionOrderItemData>();

        List<PurchaseRequisitionOrderItem> toRemoves = purchaseRequisitionOrder.PurchaseRequisitionOrderItems
            .Where(a => !finalItems.Any(i => i.SkuId == a.SkuId))
            .ToList();
        foreach (PurchaseRequisitionOrderItem item in toRemoves)
        {
            purchaseRequisitionOrder.DeleteItem(item.Id);
        }

        Dictionary<long, long> existingDict = new Dictionary<long, long>();
        foreach (PurchaseRequisitionOrderItem a in purchaseRequisitionOrder.PurchaseRequisitionOrderItems)
        {
            existingDict[a.SkuId] = a.Id;
        }

        foreach (PurchaseRequisitionOrderItemData item in finalItems)
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

            if (existingDict.TryGetValue(item.SkuId, out long id))
            {
                purchaseRequisitionOrder.ModifyItem(
                    id,
                    item.IsFirst,
                    item.Qty,
                    item.WeeklyUsage,
                    item.CurrentQty,
                    item.Purpose,
                    item.MustDeliveryOn,
                    item.DeliveryAddress,
                    item.DeliveryOn,
                    item.Remark);
            }
            else
            {
                purchaseRequisitionOrder.AddItem(
                    sku.SpuId,
                    item.SkuId,
                    item.IsFirst,
                    item.Qty,
                    item.WeeklyUsage,
                    item.CurrentQty,
                    item.Purpose,
                    item.MustDeliveryOn,
                    item.DeliveryAddress,
                    item.DeliveryOn,
                    item.Remark);
            }
        }
    }

    /// <inheritdoc />
    public async Task<PurchaseRequisitionOrder> ModifyAsync(long id, long departmentId, string? remark, CancellationToken cancellationToken = default)
    {
        PurchaseRequisitionOrder? model = await _purchaseRequisitionOrderRepository.FindAsync(id, true, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("申购单不存在!");
        }

        Department? department = await _departmentRepository.FindAsync(departmentId, true, cancellationToken);
        if (department is null)
        {
            throw new UserFriendlyException("部门" + departmentId + "不存在!");
        }

        model.Modify(departmentId, department.CompanyId, remark);
        return model;
    }

    /// <inheritdoc />
    public async Task<PurchaseRequisitionOrderAssignResult> AutomaticAssignAsync(long id, CancellationToken cancellationToken = default)
    {
        PurchaseRequisitionOrder? purchaseRequisitionOrder = await _purchaseRequisitionOrderRepository.FindAsync(id, true, cancellationToken);
        if (purchaseRequisitionOrder is null)
        {
            throw new UserFriendlyException("申购单不存在!");
        }

        LeaveStockPlan leaveStockPlan = await GetOrCreateLeaveStockPlanAsync(purchaseRequisitionOrder, cancellationToken);
        PurchasePlan purchasePlan = await GetOrCreatePurchasePlanAsync(purchaseRequisitionOrder, cancellationToken);

        List<LeaveStockPlanItemData> leaveStockPlanItems = new List<LeaveStockPlanItemData>();
        List<PurchasePlanItemData> purchasePlanItems = new List<PurchasePlanItemData>();
        foreach (PurchaseRequisitionOrderItem item in purchaseRequisitionOrder.PurchaseRequisitionOrderItems)
        {
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku is null)
            {
                continue;
            }

            if (item.Qty <= 0)
            {
                continue;
            }

            Stock? stock = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(sku.Id, DefaultWarehouseId, cancellationToken);
            double actualQty = 0;
            if (stock is not null)
            {
                actualQty = stock.GetActualQty();
            }

            double qty = actualQty - item.Qty;
            if (qty >= 0)
            {
                leaveStockPlanItems.Add(new LeaveStockPlanItemData(item.Id, item.SkuId, DefaultWarehouseId, item.Qty));
            }
            else
            {
                if (actualQty > 0)
                {
                    leaveStockPlanItems.Add(new LeaveStockPlanItemData(item.Id, item.SkuId, DefaultWarehouseId, actualQty));
                }

                purchasePlanItems.Add(new PurchasePlanItemData(item.Id, item.SkuId, DefaultWarehouseId, item.Qty - actualQty));
            }
        }

        await _leaveStockPlanDomainService.SyncLeaveStockPlanItemsAsync(leaveStockPlan, leaveStockPlanItems, cancellationToken);
        await _purchasePlanDomainService.SyncPurchasePlanItemsAsync(purchasePlan, purchasePlanItems, cancellationToken);

        return new PurchaseRequisitionOrderAssignResult(leaveStockPlan, purchasePlan);
    }

    /// <inheritdoc />
    public async Task<PurchaseRequisitionOrderAssignResult> AllCustomizedAssignAsync(long id, CancellationToken cancellationToken = default)
    {
        PurchaseRequisitionOrder? purchaseRequisitionOrder = await _purchaseRequisitionOrderRepository.FindAsync(id, true, cancellationToken);
        if (purchaseRequisitionOrder is null)
        {
            throw new UserFriendlyException("申购单不存在!");
        }

        LeaveStockPlan leaveStockPlan = await GetOrCreateLeaveStockPlanAsync(purchaseRequisitionOrder, cancellationToken);
        PurchasePlan purchasePlan = await GetOrCreatePurchasePlanAsync(purchaseRequisitionOrder, cancellationToken);

        List<PurchasePlanItemData> purchasePlanItems = new List<PurchasePlanItemData>();
        foreach (PurchaseRequisitionOrderItem item in purchaseRequisitionOrder.PurchaseRequisitionOrderItems)
        {
            Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
            if (sku is null)
            {
                continue;
            }

            if (item.Qty > 0)
            {
                purchasePlanItems.Add(new PurchasePlanItemData(item.Id, item.SkuId, DefaultWarehouseId, item.Qty));
            }
        }

        await _leaveStockPlanDomainService.SyncLeaveStockPlanItemsAsync(leaveStockPlan, null, cancellationToken);
        await _purchasePlanDomainService.SyncPurchasePlanItemsAsync(purchasePlan, purchasePlanItems, cancellationToken);

        return new PurchaseRequisitionOrderAssignResult(leaveStockPlan, purchasePlan);
    }

    /// <inheritdoc />
    public async Task<PurchaseRequisitionOrderAssignResult> AssignAsync(long id, IReadOnlyCollection<SkuInfoData> leaveStockPlanSkuInfos, IReadOnlyCollection<SkuInfoData> purchasePlanSkuInfos, CancellationToken cancellationToken = default)
    {
        PurchaseRequisitionOrder? purchaseRequisitionOrder = await _purchaseRequisitionOrderRepository.FindAsync(id, true, cancellationToken);
        if (purchaseRequisitionOrder is null)
        {
            throw new UserFriendlyException("申购单不存在!");
        }

        if (purchaseRequisitionOrder.Status != nameof(PurchaseRequisitionOrderStatusEnum.PendingPurchaseCheck))
        {
            throw new UserFriendlyException("申购单" + purchaseRequisitionOrder.Number + "当前状态不允许分配!");
        }

        LeaveStockPlan leaveStockPlan = await GetOrCreateLeaveStockPlanAsync(purchaseRequisitionOrder, cancellationToken);
        PurchasePlan purchasePlan = await GetOrCreatePurchasePlanAsync(purchaseRequisitionOrder, cancellationToken);

        List<LeaveStockPlanItemData> leaveStockPlanItems = new List<LeaveStockPlanItemData>();
        if (leaveStockPlanSkuInfos.Sum(x => x.Qty) > 0)
        {
            foreach (SkuInfoData item in leaveStockPlanSkuInfos)
            {
                if (item.Qty <= 0)
                {
                    continue;
                }

                Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
                if (sku is null)
                {
                    throw new UserFriendlyException("Sku " + item.SkuId + "不存在!");
                }

                Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
                if (spu is null)
                {
                    throw new UserFriendlyException("产品" + sku.SpuId + "不存在!");
                }

                PurchaseRequisitionOrderItem? purchaseRequisitionOrderItem = purchaseRequisitionOrder.PurchaseRequisitionOrderItems
                    .FirstOrDefault(x => x.SkuId == item.SkuId);
                if (purchaseRequisitionOrderItem is null)
                {
                    throw new UserFriendlyException("申购单" + purchaseRequisitionOrder.Number + "产品" + spu.Code + "不存在!");
                }

                if (purchaseRequisitionOrderItem.Qty < item.Qty)
                {
                    throw new UserFriendlyException("申购单" + purchaseRequisitionOrder.Number + "产品" + spu.Code + "超出可生成数量!");
                }

                leaveStockPlanItems.Add(new LeaveStockPlanItemData(purchaseRequisitionOrderItem.Id, item.SkuId, item.WarehouseId, item.Qty));
            }
        }

        List<PurchasePlanItemData> purchasePlanItems = new List<PurchasePlanItemData>();
        if (purchasePlanSkuInfos.Sum(x => x.Qty) > 0)
        {
            foreach (SkuInfoData item in purchasePlanSkuInfos)
            {
                if (item.Qty <= 0)
                {
                    continue;
                }

                Sku? sku = await _skuRepository.FindAsync(item.SkuId, true, cancellationToken);
                if (sku is null)
                {
                    throw new UserFriendlyException("Sku " + item.SkuId + "不存在!");
                }

                Spu? spu = await _spuRepository.FindAsync(sku.SpuId, true, cancellationToken);
                if (spu is null)
                {
                    throw new UserFriendlyException("产品" + sku.SpuId + "不存在!");
                }

                PurchaseRequisitionOrderItem? purchaseRequisitionOrderItem = purchaseRequisitionOrder.PurchaseRequisitionOrderItems
                    .FirstOrDefault(x => x.SkuId == item.SkuId);
                if (purchaseRequisitionOrderItem is null)
                {
                    throw new UserFriendlyException("申购单" + purchaseRequisitionOrder.Number + "产品" + spu.Code + "不存在!");
                }

                double otherQty = leaveStockPlanItems.Where(x => x.SkuId == sku.Id).Sum(x => x.Qty);
                if (purchaseRequisitionOrderItem.Qty - otherQty < item.Qty)
                {
                    throw new UserFriendlyException("申购单" + purchaseRequisitionOrder.Number + "产品" + spu.Code + "超出可生成数量!");
                }

                purchasePlanItems.Add(new PurchasePlanItemData(purchaseRequisitionOrderItem.Id, item.SkuId, item.WarehouseId, item.Qty));
            }
        }

        await _leaveStockPlanDomainService.SyncLeaveStockPlanItemsAsync(leaveStockPlan, leaveStockPlanItems, cancellationToken);
        await _purchasePlanDomainService.SyncPurchasePlanItemsAsync(purchasePlan, purchasePlanItems, cancellationToken);

        return new PurchaseRequisitionOrderAssignResult(leaveStockPlan, purchasePlan);
    }

    /// <summary>
    /// 查出库计划，不存在则生成
    /// </summary>
    /// <param name="purchaseRequisitionOrder">申购单</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出库计划</returns>
    private async Task<LeaveStockPlan> GetOrCreateLeaveStockPlanAsync(PurchaseRequisitionOrder purchaseRequisitionOrder, CancellationToken cancellationToken)
    {
        LeaveStockPlan? leaveStockPlan = await _leaveStockPlanRepository.GetByRelationAsync(nameof(BusinessOrderTypeEnum.PurchaseRequisitionOrder), purchaseRequisitionOrder.Id, null, cancellationToken);
        if (leaveStockPlan is not null)
        {
            return leaveStockPlan;
        }

        return await _leaveStockPlanFactory.CreateAsync(
            nameof(ProductTypeEnum.Accessory),
            purchaseRequisitionOrder.DepartmentId,
            purchaseRequisitionOrder.CompanyId,
            nameof(BusinessOrderTypeEnum.PurchaseRequisitionOrder),
            purchaseRequisitionOrder.Id,
            purchaseRequisitionOrder.Number,
            purchaseRequisitionOrder.Remark,
            cancellationToken);
    }

    /// <summary>
    /// 查采购计划，不存在则生成
    /// </summary>
    /// <param name="purchaseRequisitionOrder">申购单</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购计划</returns>
    private async Task<PurchasePlan> GetOrCreatePurchasePlanAsync(PurchaseRequisitionOrder purchaseRequisitionOrder, CancellationToken cancellationToken)
    {
        PurchasePlan? purchasePlan = await _purchasePlanRepository.GetByRelationAsync(nameof(BusinessOrderTypeEnum.PurchaseRequisitionOrder), purchaseRequisitionOrder.Id, null, cancellationToken);
        if (purchasePlan is not null)
        {
            return purchasePlan;
        }

        return await _purchasePlanFactory.CreateAsync(
            nameof(ProductTypeEnum.Accessory),
            purchaseRequisitionOrder.DepartmentId,
            purchaseRequisitionOrder.CompanyId,
            nameof(BusinessOrderTypeEnum.PurchaseRequisitionOrder),
            purchaseRequisitionOrder.Id,
            purchaseRequisitionOrder.Number,
            purchaseRequisitionOrder.Remark,
            cancellationToken);
    }
}
