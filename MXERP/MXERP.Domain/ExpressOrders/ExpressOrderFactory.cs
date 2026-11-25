using Snow.DependencyInjection;
using Snow.Exceptions;

namespace MXERP.Domain.ExpressOrders;

/// <summary>
/// 快递单工厂
/// </summary>
[ScopedService]
public class ExpressOrderFactory : IExpressOrderFactory
{
    private readonly IExpressOrderRepository _expressOrderRepository;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="expressOrderRepository">快递单仓储</param>
    public ExpressOrderFactory(IExpressOrderRepository expressOrderRepository)
    {
        _expressOrderRepository = expressOrderRepository;
    }

    /// <inheritdoc />
    public async Task<ExpressOrder> CreateAsync(string number, string customerCode, CancellationToken cancellationToken = default)
    {
        bool isExist = await _expressOrderRepository.IsExistByNumberExcludeIdAsync(number, null, cancellationToken);
        if (isExist)
        {
            throw new UserFriendlyException("快递单号" + number + "已存在!");
        }

        return new ExpressOrder(number, customerCode);
    }
}
