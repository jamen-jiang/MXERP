using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.EnterStockOrders;

/// <summary>
/// 入库单工厂
/// </summary>
[ScopedService]
public class EnterStockOrderFactory : IEnterStockOrderFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public EnterStockOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <summary>
    /// 创建入库单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="originalId">原始Id（可空）</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单聚合</returns>
    public async Task<EnterStockOrder> CreateAsync(string productType, long? originalId, string businessType, string? remark, CancellationToken cancellationToken = default)
    {
        EnterStockOrder model = new EnterStockOrder(productType, originalId, businessType, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }

    /// <summary>
    /// 创建入库单（带关联类型）
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="originalId">原始Id（可空）</param>
    /// <param name="businessOrderType">业务订单类型（关联类型）</param>
    /// <param name="businessType">业务类型</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>入库单聚合</returns>
    public async Task<EnterStockOrder> CreateAsync(string productType, long? originalId, string businessOrderType, string businessType, string? remark, CancellationToken cancellationToken = default)
    {
        EnterStockOrder model = new EnterStockOrder(productType, originalId, businessOrderType, businessType, remark);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }
}
