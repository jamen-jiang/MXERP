using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.LeaveWarehouseOrders;

/// <summary>
/// 出仓单工厂接口
/// </summary>
public interface ILeaveWarehouseOrderFactory
{
    /// <summary>
    /// 创建出仓单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="type">类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="sendCompanyId">发货公司Id</param>
    /// <param name="sender">发货人</param>
    /// <param name="senderTelephone">发货人电话(可空)</param>
    /// <param name="sendArea">发货地区(可空)</param>
    /// <param name="sendAddress">发货地址(可空)</param>
    /// <param name="deliveryCompanyId">收货公司Id</param>
    /// <param name="deliveryer">收货人</param>
    /// <param name="deliveryerTelephone">收货人电话(可空)</param>
    /// <param name="deliveryArea">收货地区(可空)</param>
    /// <param name="deliveryAddress">收货地址(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>出仓单聚合</returns>
    Task<LeaveWarehouseOrder> CreateAsync(ProductTypeEnum productType, LeaveWarehouseOrderTypeEnum type, long companyId, long sendCompanyId, string sender, string? senderTelephone, string? sendArea, string? sendAddress, long deliveryCompanyId, string deliveryer, string? deliveryerTelephone, string? deliveryArea, string? deliveryAddress, CancellationToken cancellationToken = default);
}
