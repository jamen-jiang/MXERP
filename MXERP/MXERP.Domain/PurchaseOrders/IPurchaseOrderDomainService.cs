using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MXERP.Domain.PurchaseOrders;

/// <summary>
/// 采购订单领域服务接口
/// </summary>
public interface IPurchaseOrderDomainService
{
    /// <summary>
    /// 从采购批次生成采购订单
    /// </summary>
    /// <param name="companyId">公司Id</param>
    /// <param name="positionId">仓位Id(可空)</param>
    /// <param name="deliveryOn">收货时间(可空)</param>
    /// <param name="deliveryer">收货人(可空)</param>
    /// <param name="deliveryerTelephone">收货人电话(可空)</param>
    /// <param name="deliveryArea">收货地区(可空)</param>
    /// <param name="deliveryAddress">收货地址(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="datas">采购批次生成参数列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采购订单列表</returns>
    Task<List<PurchaseOrder>> GenerateFromBatchesAsync(
        long companyId,
        long? positionId,
        DateTime? deliveryOn,
        string? deliveryer,
        string? deliveryerTelephone,
        string? deliveryArea,
        string? deliveryAddress,
        string? remark,
        IReadOnlyList<GeneratePurchaseOrderFromBatchData> datas,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改采购订单项列表(价格/税率/折扣)
    /// </summary>
    /// <param name="purchaseOrder">采购订单聚合</param>
    /// <param name="skuInfos">SKU信息列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task ModifyItemsAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderItemData> skuInfos, CancellationToken cancellationToken = default);
}
