using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.DependencyInjection;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单工厂
/// </summary>
[ScopedService]
public class ExternalLeaveStockOrderFactory : IExternalLeaveStockOrderFactory
{
    /// <summary>
    /// 流水号生成器
    /// </summary>
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="serialNumberGenerator">流水号生成器</param>
    public ExternalLeaveStockOrderFactory(ISerialNumberGenerator serialNumberGenerator)
    {
        _serialNumberGenerator = serialNumberGenerator;
    }

    /// <inheritdoc />
    public async Task<ExternalLeaveStockOrder> CreateAsync(
        ProductTypeEnum productType,
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
        CancellationToken cancellationToken = default)
    {
        ExternalLeaveStockOrder model = new ExternalLeaveStockOrder(
            productType,
            storeId,
            companyId,
            customerOrderNumber,
            customerPONumber,
            salesSpuId,
            salesSpuCode,
            spuId,
            orderQty,
            qty,
            songCangKu,
            faLiao,
            kouZhang,
            songBaoZhuang,
            etc,
            daMai,
            huangYou,
            daYinTiaoMa,
            huanJingDai,
            tieBeiTie,
            daBao,
            zhuangXiangMingXi,
            chuHuoJianCha,
            fengXiang,
            orderOn,
            deliveryOn,
            remark);

        string number = await _serialNumberGenerator.NextAsync(model.GetSerialNumberType(), cancellationToken);
        model.AssignNumber(number);

        return model;
    }
}
