using MXERP.Domain.Skus;
using MXERP.Domain.Spus;
using MXERP.Domain.Warehouses;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.Stocks;

/// <summary>
/// 库存领域服务, 负责库存增减、冻结、解冻
/// </summary>
[ScopedService]
public class StockDomainService : IStockDomainService
{
    private readonly IStockRepository _stockRepository;
    private readonly ISkuRepository _skuRepository;
    private readonly ISpuRepository _spuRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="stockRepository">库存仓储</param>
    /// <param name="skuRepository">Sku仓储</param>
    /// <param name="spuRepository">Spu仓储</param>
    /// <param name="warehouseRepository">仓库仓储</param>
    public StockDomainService(IStockRepository stockRepository, ISkuRepository skuRepository, ISpuRepository spuRepository, IWarehouseRepository warehouseRepository)
    {
        _stockRepository = stockRepository;
        _skuRepository = skuRepository;
        _spuRepository = spuRepository;
        _warehouseRepository = warehouseRepository;
    }

    /// <inheritdoc />
    public async Task<Stock?> AddQtyAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default)
    {
        if (qty == 0)
        {
            return null;
        }

        Stock? model = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(skuId, warehouseId, cancellationToken);
        if (model is null)
        {
            Sku? sku = await _skuRepository.FindAsync(skuId, true, cancellationToken);
            if (sku is null)
            {
                throw new UserFriendlyException("Sku不存在!");
            }

            model = new Stock(sku.ProductType, sku.SpuId, skuId, warehouseId, sku.KeyAttributes);
        }

        string spuCode = await FindSpuCodeAsync(model.SpuId, cancellationToken);
        string warehouseCode = await FindWarehouseCodeAsync(warehouseId, cancellationToken);
        model.AddQty(qty, spuCode, warehouseCode);
        return model;
    }

    /// <inheritdoc />
    public async Task<Stock?> SubtractQtyAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default)
    {
        if (qty == 0)
        {
            return null;
        }

        Stock? model = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(skuId, warehouseId, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("库存不存在!");
        }

        string spuCode = await FindSpuCodeAsync(model.SpuId, cancellationToken);
        string warehouseCode = await FindWarehouseCodeAsync(warehouseId, cancellationToken);
        model.SubtractQty(qty, spuCode, warehouseCode);
        return model;
    }

    /// <inheritdoc />
    public async Task<Stock?> FreezeStockAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default)
    {
        if (qty == 0)
        {
            return null;
        }

        Stock? model = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(skuId, warehouseId, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("库存不存在!");
        }

        string spuCode = await FindSpuCodeAsync(model.SpuId, cancellationToken);
        string warehouseCode = await FindWarehouseCodeAsync(warehouseId, cancellationToken);
        model.FreezeStock(qty, spuCode, warehouseCode);
        return model;
    }

    /// <inheritdoc />
    public async Task<Stock?> FreezeStockAgainAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default)
    {
        if (qty == 0)
        {
            return null;
        }

        Stock? model = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(skuId, warehouseId, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("库存不存在!");
        }

        model.FreezeStockAgain(qty);
        return model;
    }

    /// <inheritdoc />
    public async Task<Stock?> ThawStockAsync(long skuId, long warehouseId, double qty, CancellationToken cancellationToken = default)
    {
        if (qty == 0)
        {
            return null;
        }

        Stock? model = await _stockRepository.GetBySkuIdAndWarehouseIdAsync(skuId, warehouseId, cancellationToken);
        if (model is null)
        {
            throw new UserFriendlyException("库存不存在!");
        }

        string spuCode = await FindSpuCodeAsync(model.SpuId, cancellationToken);
        string warehouseCode = await FindWarehouseCodeAsync(warehouseId, cancellationToken);
        model.ThawStock(qty, spuCode, warehouseCode);
        return model;
    }

    /// <summary>
    /// 查询Spu代码
    /// </summary>
    /// <param name="spuId">产品Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>产品代码, 不存在返回"未知"</returns>
    private async Task<string> FindSpuCodeAsync(long spuId, CancellationToken cancellationToken)
    {
        Spu? spu = await _spuRepository.FindAsync(spuId, true, cancellationToken);
        if (spu is null)
        {
            return "未知";
        }

        return spu.Code;
    }

    /// <summary>
    /// 查询仓库代码
    /// </summary>
    /// <param name="warehouseId">仓库Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>仓库代码, 不存在返回"未知"</returns>
    private async Task<string> FindWarehouseCodeAsync(long warehouseId, CancellationToken cancellationToken)
    {
        Warehouse? warehouse = await _warehouseRepository.FindAsync(warehouseId, true, cancellationToken);
        if (warehouse is null)
        {
            return "未知";
        }

        return warehouse.Code;
    }
}
