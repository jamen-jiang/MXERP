using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单工厂
/// </summary>
[ScopedService]
public class RetailOrderFactory : IRetailOrderFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public RetailOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<RetailOrder> CreateAsync(long storeId, long companyId, string? outArea, string? sequenceNumber, string? hkNumber, string customerOrderNumber, string? customerPONumber, string? documentaryBarCode, int outGood, string? outGoodTime, string? material, string? coating, string? tintingColor, string? mirror, string? tintingCode, string? tintingName, bool? uv, bool? hard, bool? polish, string? auro, string? edge, string? superHole, string? special, string? specialCode, string? zuanKong, string? frame, string? remark, CancellationToken cancellationToken = default)
    {
        RetailOrder model = new RetailOrder(storeId, companyId, outArea, sequenceNumber, hkNumber, customerOrderNumber, customerPONumber, documentaryBarCode, outGood, outGoodTime, material, coating, tintingColor, mirror, tintingCode, tintingName, uv, hard, polish, auro, edge, superHole, special, specialCode, zuanKong, frame, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }
}
