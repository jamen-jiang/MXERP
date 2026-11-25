using System.Threading;
using System.Threading.Tasks;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.PurchaseRequisitionOrders;

/// <summary>
/// 申购单工厂
/// </summary>
[ScopedService]
public class PurchaseRequisitionOrderFactory : IPurchaseRequisitionOrderFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public PurchaseRequisitionOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<PurchaseRequisitionOrder> CreateAsync(long departmentId, long companyId, string? remark, CancellationToken cancellationToken = default)
    {
        PurchaseRequisitionOrder model = new PurchaseRequisitionOrder(departmentId, companyId, remark);

        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);

        return model;
    }
}
