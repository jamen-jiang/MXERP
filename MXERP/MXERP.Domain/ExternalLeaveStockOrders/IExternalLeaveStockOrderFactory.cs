using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单工厂接口
/// </summary>
public interface IExternalLeaveStockOrderFactory
{
    /// <summary>
    /// 创建外部出库单
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号（可空）</param>
    /// <param name="salesSpuId">销售产品Id</param>
    /// <param name="salesSpuCode">销售产品Code</param>
    /// <param name="spuId">产品Id</param>
    /// <param name="orderQty">来单数量</param>
    /// <param name="qty">出库数量</param>
    /// <param name="songCangKu">送仓库</param>
    /// <param name="faLiao">发料</param>
    /// <param name="kouZhang">扣账</param>
    /// <param name="songBaoZhuang">送包装</param>
    /// <param name="etc">ETC</param>
    /// <param name="daMai">打唛</param>
    /// <param name="huangYou">黄油</param>
    /// <param name="daYinTiaoMa">打印条码</param>
    /// <param name="huanJingDai">换镜袋</param>
    /// <param name="tieBeiTie">贴背贴</param>
    /// <param name="daBao">打包</param>
    /// <param name="zhuangXiangMingXi">装箱明细</param>
    /// <param name="chuHuoJianCha">出货检查</param>
    /// <param name="fengXiang">封箱</param>
    /// <param name="orderOn">订单时间</param>
    /// <param name="deliveryOn">预出货时间（可空）</param>
    /// <param name="remark">备注（可空）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>外部出库单聚合</returns>
    Task<ExternalLeaveStockOrder> CreateAsync(
        string productType,
        long storeId,
        long companyId,
        string customerOrderNumber,
        string? customerPONumber,
        long salesSpuId,
        string salesSpuCode,
        long spuId,
        int orderQty,
        int qty,
        bool songCangKu,
        bool faLiao,
        bool kouZhang,
        bool songBaoZhuang,
        bool etc,
        bool daMai,
        bool huangYou,
        bool daYinTiaoMa,
        bool huanJingDai,
        bool tieBeiTie,
        bool daBao,
        bool zhuangXiangMingXi,
        bool chuHuoJianCha,
        bool fengXiang,
        DateTime orderOn,
        DateTime? deliveryOn,
        string? remark,
        CancellationToken cancellationToken = default);
}
