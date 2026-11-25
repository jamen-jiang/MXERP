using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单领域服务, 负责跨聚合编排和项同步
/// </summary>
[ScopedService]
public class ExpressOrderDomainService : IExpressOrderDomainService
{
    private readonly IExpressOrderFactory _expressOrderFactory;
    private readonly IExpressOrderRepository _expressOrderRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="expressOrderFactory">快递单工厂</param>
    /// <param name="expressOrderRepository">快递单仓储</param>
    public ExpressOrderDomainService(IExpressOrderFactory expressOrderFactory, IExpressOrderRepository expressOrderRepository)
    {
        _expressOrderFactory = expressOrderFactory;
        _expressOrderRepository = expressOrderRepository;
    }

    /// <inheritdoc />
    public async Task<ExpressOrder> ModifyAsync(long id, string customerCode, CancellationToken cancellationToken = default)
    {
        ExpressOrder? expressOrder = await _expressOrderRepository.FindAsync(id, true, cancellationToken);
        if (expressOrder is null)
        {
            throw new UserFriendlyException("快递单不存在!");
        }

        Modify(expressOrder, customerCode);
        return expressOrder;
    }

    /// <inheritdoc />
    public void Modify(ExpressOrder expressOrder, string customerCode)
    {
        expressOrder.Modify(customerCode);
    }

    /// <inheritdoc />
    public async Task<ExpressOrder> ImportGenerateAsync(string number, string customerCode, CancellationToken cancellationToken = default)
    {
        ExpressOrder? expressOrder = await _expressOrderRepository.GetByNumberAsync(number, cancellationToken);
        if (expressOrder is null)
        {
            return await _expressOrderFactory.CreateAsync(number, customerCode, cancellationToken);
        }

        Modify(expressOrder, customerCode);
        return expressOrder;
    }
}
