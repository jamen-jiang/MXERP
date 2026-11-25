using MXERP.Domain.Stores;
using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.PackingSlips;

/// <summary>
/// 装箱单领域服务
/// </summary>
[ScopedService]
public class PackingSlipDomainService : IPackingSlipDomainService
{
    private readonly IPackingSlipRepository _packingSlipRepository;
    private readonly IStoreRepository _storeRepository;

    /// <summary>
    /// 供应商代码
    /// </summary>
    private const string SupplierCode = "39060";

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="packingSlipRepository">装箱单仓储</param>
    /// <param name="storeRepository">门店仓储</param>
    public PackingSlipDomainService(IPackingSlipRepository packingSlipRepository, IStoreRepository storeRepository)
    {
        _packingSlipRepository = packingSlipRepository;
        _storeRepository = storeRepository;
    }

    /// <inheritdoc />
    public async Task<PackingSlip> CreateAsync(PackingSlipData data, CancellationToken cancellationToken = default)
    {
        if (await _packingSlipRepository.IsExistCustomerOrderNumberAsync(data.CustomerOrderNumber, cancellationToken))
        {
            throw new UserFriendlyException("客户单号" + data.CustomerOrderNumber + "已存在!");
        }
        return await GenerateAsync(data, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PackingSlip> ImportGenerateAsync(PackingSlipData data, long currentUserId, string currentUserName, CancellationToken cancellationToken = default)
    {
        PackingSlip? model = await _packingSlipRepository.GetByCustomerOrderNumberAsync(data.CustomerOrderNumber, cancellationToken);
        if (model == null)
        {
            return await GenerateAsync(data, cancellationToken);
        }
        model.Update(currentUserId, currentUserName);
        return model;
    }

    /// <summary>
    /// 生成装箱单（查门店、生成二维码、构造聚合，不做唯一性校验）
    /// </summary>
    /// <param name="data">装箱单数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>装箱单聚合</returns>
    private async Task<PackingSlip> GenerateAsync(PackingSlipData data, CancellationToken cancellationToken)
    {
        Store? store = await _storeRepository.GetByCodeAsync(data.StoreCode, cancellationToken);
        if (store == null)
        {
            throw new UserFriendlyException("门店" + data.StoreCode + "不存在!");
        }
        if (string.IsNullOrWhiteSpace(store.CityCode))
        {
            throw new UserFriendlyException("门店" + data.StoreCode + "区号为空!");
        }

        int cartonNumber = 1;
        string qrCode = SupplierCode + store.Code + store.CityCode + cartonNumber.ToString("D2");

        return new PackingSlip(qrCode, cartonNumber, store.Id, data);
    }
}
