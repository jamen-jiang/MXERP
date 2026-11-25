using MXERP.Domain.LeaveStockBatches;
using MXERP.Domain.SalesSpus;
using MXERP.Domain.Spus;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单领域服务
/// </summary>
[ScopedService]
public class LeaveWarehouseOrderDomainService : ILeaveWarehouseOrderDomainService
{
    /// <summary>
    /// 出仓单仓储
    /// </summary>
    private readonly ILeaveWarehouseOrderRepository _leaveWarehouseOrderRepository;
    /// <summary>
    /// 销售产品仓储
    /// </summary>
    private readonly ISalesSpuRepository _salesSpuRepository;
    /// <summary>
    /// 产品仓储
    /// </summary>
    private readonly ISpuRepository _spuRepository;
    /// <summary>
    /// 出库批次仓储
    /// </summary>
    private readonly ILeaveStockBatchRepository _leaveStockBatchRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="leaveWarehouseOrderRepository">出仓单仓储</param>
    /// <param name="salesSpuRepository">销售产品仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="leaveStockBatchRepository">出库批次仓储</param>
    public LeaveWarehouseOrderDomainService(ILeaveWarehouseOrderRepository leaveWarehouseOrderRepository, ISalesSpuRepository salesSpuRepository, ISpuRepository spuRepository, ILeaveStockBatchRepository leaveStockBatchRepository)
    {
        _leaveWarehouseOrderRepository = leaveWarehouseOrderRepository;
        _salesSpuRepository = salesSpuRepository;
        _spuRepository = spuRepository;
        _leaveStockBatchRepository = leaveStockBatchRepository;
    }

    /// <inheritdoc />
    public async Task SyncLeaveWarehouseOrderItemsAsync(LeaveWarehouseOrder leaveWarehouseOrder, IReadOnlyCollection<LeaveWarehouseOrderItemData>? leaveWarehouseOrderItems, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<LeaveWarehouseOrderItemData> finalItems = leaveWarehouseOrderItems ?? new List<LeaveWarehouseOrderItemData>();

        List<LeaveWarehouseOrderItem> toRemoves = leaveWarehouseOrder.LeaveWarehouseOrderItems
            .Where(a => !finalItems.Any(i => i.LeaveStockBatchId == a.LeaveStockBatchId && i.SalesSpuId == a.SalesSpuId && i.SpuId == a.SpuId))
            .ToList();

        foreach (LeaveWarehouseOrderItem item in toRemoves)
        {
            leaveWarehouseOrder.DeleteItem(item.Id);
        }

        Dictionary<BusinessKey, long> existingDict = new Dictionary<BusinessKey, long>();
        foreach (LeaveWarehouseOrderItem a in leaveWarehouseOrder.LeaveWarehouseOrderItems)
        {
            existingDict[new BusinessKey(a.LeaveStockBatchId, a.SalesSpuId, a.SpuId)] = a.Id;
        }

        foreach (LeaveWarehouseOrderItemData item in finalItems)
        {
            if (item.Qty <= 0)
            {
                throw new UserFriendlyException("出仓单" + leaveWarehouseOrder.Number + "出仓单项出仓数量不允许小于等于0!");
            }

            SalesSpu? salesSpu = null;
            if (item.SalesSpuId is not null)
            {
                salesSpu = await _salesSpuRepository.FindAsync(item.SalesSpuId.Value, true, cancellationToken);
            }

            Spu? spu = await _spuRepository.FindAsync(item.SpuId, true, cancellationToken);
            if (spu is null)
            {
                throw new UserFriendlyException("产品 " + item.SpuId + "不存在!");
            }

            LeaveStockBatch? leaveStockBatch = await _leaveStockBatchRepository.FindAsync(item.LeaveStockBatchId, true, cancellationToken);
            if (leaveStockBatch is null)
            {
                throw new UserFriendlyException("出库批次" + item.LeaveStockBatchId + "不存在!");
            }

            string? salesSpuCode = null;
            if (salesSpu is not null)
            {
                salesSpuCode = salesSpu.Code;
            }

            decimal price = spu.PurchasePrice;
            if (salesSpu is not null)
            {
                price = salesSpu.Price;
            }

            leaveWarehouseOrder.AddItem(leaveStockBatch.Id, leaveStockBatch.Number, item.SalesSpuId, salesSpuCode, spu.Id, item.Qty, price, item.Remark);
        }
    }

    /// <summary>
    /// 出仓单项业务标识(出库批次 + 销售产品 + 产品, 用于去重)
    /// </summary>
    /// <param name="LeaveStockBatchId">出库批次Id</param>
    /// <param name="SalesSpuId">销售产品Id(可空)</param>
    /// <param name="SpuId">产品Id</param>
    private sealed record BusinessKey(long LeaveStockBatchId, long? SalesSpuId, long SpuId);
}
