using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单工厂
/// </summary>
[ScopedService]
public class LeaveWarehouseOrderFactory : ILeaveWarehouseOrderFactory
{
    /// <summary>
    /// 流水号生成器
    /// </summary>
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public LeaveWarehouseOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<LeaveWarehouseOrder> CreateAsync(ProductTypeEnum productType, LeaveWarehouseOrderTypeEnum type, long companyId, long sendCompanyId, string sender, string? senderTelephone, string? sendArea, string? sendAddress, long deliveryCompanyId, string deliveryer, string? deliveryerTelephone, string? deliveryArea, string? deliveryAddress, CancellationToken cancellationToken = default)
    {
        LeaveWarehouseOrder model = new LeaveWarehouseOrder(productType, type, companyId, sendCompanyId, sender, senderTelephone, sendArea, sendAddress, deliveryCompanyId, deliveryer, deliveryerTelephone, deliveryArea, deliveryAddress);

        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);

        return model;
    }
}
