using MXERP.Domain.Common.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购单工厂接口
/// </summary>
public interface IPurchaseOrderFactory
{
    /// <summary>
    /// 创建采购单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="relationType">关联类型</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="positionId">仓位Id(可空)</param>
    /// <param name="supplierId">供应商Id</param>
    /// <param name="payMode">支付方式(可空)</param>
    /// <param name="deliveryMode">收货方式(可空)</param>
    /// <param name="deliveryOn">收货时间(可空)</param>
    /// <param name="deliveryer">收货人</param>
    /// <param name="deliveryerTelephone">收货人电话</param>
    /// <param name="deliveryArea">收货地区</param>
    /// <param name="deliveryAddress">收货地址</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购单聚合</returns>
    Task<PurchaseOrder> CreateAsync(
        ProductTypeEnum productType,
        BusinessOrderTypeEnum relationType,
        long companyId,
        long? positionId,
        long supplierId,
        string? payMode,
        string? deliveryMode,
        DateTime? deliveryOn,
        string deliveryer,
        string deliveryerTelephone,
        string deliveryArea,
        string deliveryAddress,
        string? remark,
        CancellationToken cancellationToken = default);
}
