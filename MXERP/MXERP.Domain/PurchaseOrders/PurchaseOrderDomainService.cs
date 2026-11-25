using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.PurchaseBatches;
using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using MXERP.Domain.Suppliers;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购订单领域服务
/// </summary>
[ScopedService]
public class PurchaseOrderDomainService : IPurchaseOrderDomainService
{
    private readonly IPurchaseBatchRepository _purchaseBatchRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IPurchaseOrderFactory _purchaseOrderFactory;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="purchaseBatchRepository">采购批次仓储</param>
    /// <param name="skuRepository">产品Sku仓储</param>
    /// <param name="spuRepository">产品仓储</param>
    /// <param name="supplierRepository">供应商仓储</param>
    /// <param name="purchaseOrderFactory">采购单工厂</param>
    public PurchaseOrderDomainService(
        IPurchaseBatchRepository purchaseBatchRepository,
        ISkuRepository skuRepository,
        ISpuRepository spuRepository,
        ISupplierRepository supplierRepository,
        IPurchaseOrderFactory purchaseOrderFactory)
    {
        _purchaseBatchRepository = purchaseBatchRepository;
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _supplierRepository = supplierRepository;
        _purchaseOrderFactory = purchaseOrderFactory;
    }

    /// <inheritdoc />
    public async Task<List<PurchaseOrder>> GenerateFromBatchesAsync(
        long companyId,
        long? positionId,
        DateTime? deliveryOn,
        string? deliveryer,
        string? deliveryerTelephone,
        string? deliveryArea,
        string? deliveryAddress,
        string? remark,
        IReadOnlyList<GeneratePurchaseOrderFromBatchData> datas,
        CancellationToken cancellationToken = default)
    {
        List<long> purchaseBatchIds = datas.Select(x => x.PurchaseBatchId).Distinct().ToList();
        List<PurchaseBatch> purchaseBatchs = await _purchaseBatchRepository.GetListByIdsAsync(purchaseBatchIds, cancellationToken);
        if (purchaseBatchs.Count == 0)
        {
            return new List<PurchaseOrder>();
        }

        string productType = purchaseBatchs[0].ProductType;

        List<long> skuIds = datas.SelectMany(x => x.SkuInfos).Select(x => x.SkuId).Distinct().ToList();
        List<Sku> skus = await _skuRepository.GetListByIdsAsync(skuIds, cancellationToken);

        List<long> spuIds = skus.Select(x => x.SpuId).Distinct().ToList();
        List<Spu> spus = await _spuRepository.GetByIdsAsync(spuIds, cancellationToken);

        List<long> supplierIds = spus.Select(x => x.SupplierId).Distinct().ToList();

        List<PurchaseOrder> list = new List<PurchaseOrder>();

        foreach (long supplierId in supplierIds)
        {
            Supplier? supplier = await _supplierRepository.FindAsync(supplierId, true, cancellationToken);
            if (supplier is null)
            {
                throw new UserFriendlyException("供应商" + supplierId + "不存在!");
            }

            foreach (Spu spu in spus.Where(x => x.SupplierId == supplierId).ToList())
            {
                List<long> skuIdsTemp = skus.Where(x => x.SpuId == spu.Id).Select(x => x.Id).ToList();

                List<GeneratePurchaseOrderFromBatchData> datasTemp = datas
                    .Where(x => x.SkuInfos.Any(a => skuIdsTemp.Contains(a.SkuId)))
                    .ToList();

                List<long> warehouseIds = datasTemp
                    .SelectMany(x => x.SkuInfos)
                    .Where(x => skuIdsTemp.Contains(x.SkuId))
                    .Select(x => x.WarehouseId)
                    .Distinct()
                    .ToList();

                foreach (long warehouseId in warehouseIds)
                {
                    PurchaseOrder model = await _purchaseOrderFactory.CreateAsync(
                        productType,
                        BusinessOrderTypeEnum.PurchaseBatch,
                        companyId,
                        positionId,
                        supplierId,
                        supplier.PayMode,
                        supplier.DeliveryMode,
                        deliveryOn,
                        deliveryer ?? string.Empty,
                        deliveryerTelephone ?? string.Empty,
                        deliveryArea ?? string.Empty,
                        deliveryAddress ?? string.Empty,
                        remark,
                        cancellationToken);

                    foreach (GeneratePurchaseOrderFromBatchData data in datasTemp
                        .Where(x => x.SkuInfos.Any(a => skuIdsTemp.Contains(a.SkuId) && a.WarehouseId == warehouseId))
                        .ToList())
                    {
                        PurchaseBatch? purchaseBatch = purchaseBatchs.FirstOrDefault(x => x.Id == data.PurchaseBatchId);
                        if (purchaseBatch is null)
                        {
                            continue;
                        }

                        foreach (PurchaseOrderItemData skuInfo in data.SkuInfos
                            .Where(x => skuIdsTemp.Contains(x.SkuId) && x.WarehouseId == warehouseId && x.Qty > 0)
                            .ToList())
                        {
                            PurchaseBatchItem? item = purchaseBatch.GetPurchaseBatchItems()
                                .FirstOrDefault(x => x.SkuId == skuInfo.SkuId && x.WarehouseId == warehouseId);
                            if (item is null)
                            {
                                throw new UserFriendlyException("采购批次" + purchaseBatch.Number + "产品Sku " + skuInfo.SkuId + "仓库" + warehouseId + "不存在!");
                            }

                            if (item.Qty - item.PurchaseQty - skuInfo.Qty < 0)
                            {
                                throw new UserFriendlyException("采购批次" + purchaseBatch.Number + "产品Sku " + skuInfo.SkuId + "仓库" + warehouseId + "超出可生成数量!");
                            }

                            model.AddItem(
                                BusinessOrderTypeEnum.PurchaseBatch,
                                purchaseBatch.Id,
                                item.Id,
                                purchaseBatch.Number,
                                spu.Id,
                                skuInfo.SkuId,
                                skuInfo.WarehouseId,
                                skuInfo.Qty,
                                skuInfo.Price,
                                skuInfo.TaxRate,
                                skuInfo.Discount,
                                item.SkuAttributes,
                                item.OtherAttributes);
                        }
                    }

                    if (model.Qty > 0)
                    {
                        list.Add(model);
                    }
                }
            }
        }

        return list;
    }

    /// <inheritdoc />
    public Task ModifyItemsAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderItemData> skuInfos, CancellationToken cancellationToken = default)
    {
        foreach (PurchaseOrderItem item in purchaseOrder.GetPurchaseOrderItems())
        {
            PurchaseOrderItemData? purchaseOrderItem = skuInfos
                .FirstOrDefault(x => x.SkuId == item.SkuId && x.WarehouseId == item.WarehouseId);
            if (purchaseOrderItem != null)
            {
                purchaseOrder.ModifyItem(item.Id, purchaseOrderItem.Price, purchaseOrderItem.TaxRate, purchaseOrderItem.Discount);
            }
        }

        return Task.CompletedTask;
    }
}
