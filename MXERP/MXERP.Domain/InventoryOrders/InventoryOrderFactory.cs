using MXERP.Domain.Common.Enums;
using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.InventoryOrders;

/// <summary>
/// 盘点单工厂
/// </summary>
[ScopedService]
public class InventoryOrderFactory : IInventoryOrderFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public InventoryOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<InventoryOrder> CreateAsync(string productType, string? remark, CancellationToken cancellationToken = default)
    {
        InventoryOrder model = new InventoryOrder(productType, remark);

        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);

        return model;
    }
}
