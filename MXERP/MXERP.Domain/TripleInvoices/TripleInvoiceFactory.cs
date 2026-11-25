using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.TripleInvoices;

/// <summary>
/// 三联发票工厂
/// </summary>
[ScopedService]
public class TripleInvoiceFactory : ITripleInvoiceFactory
{
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public TripleInvoiceFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<TripleInvoice> CreateAsync(TripleInvoiceData data, CancellationToken cancellationToken = default)
    {
        TripleInvoice model = new TripleInvoice(data);
        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);
        return model;
    }
}
