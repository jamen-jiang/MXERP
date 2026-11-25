using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.LeaveStockOrders;

/// <summary>
/// 出库单工厂
/// </summary>
[ScopedService]
public class LeaveStockOrderFactory : ILeaveStockOrderFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public LeaveStockOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder> CreateAsync(ProductTypeEnum productType, long? originalId, LeaveStockBusinessTypeEnum businessType, string? remark, CancellationToken cancellationToken = default)
    {
        LeaveStockOrder model = new LeaveStockOrder(productType, originalId, businessType, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }

    /// <inheritdoc />
    public async Task<LeaveStockOrder> CreateAsync(ProductTypeEnum productType, long? originalId, BusinessOrderTypeEnum businessOrderType, LeaveStockBusinessTypeEnum businessType, string? remark, CancellationToken cancellationToken = default)
    {
        LeaveStockOrder model = new LeaveStockOrder(productType, originalId, businessOrderType, businessType, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }
}
