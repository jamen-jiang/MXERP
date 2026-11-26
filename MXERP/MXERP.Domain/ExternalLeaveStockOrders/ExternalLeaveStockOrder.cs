using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.Common.Enums;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.ExternalLeaveStockOrders;

/// <summary>
/// 外部出库单
/// </summary>
public class ExternalLeaveStockOrder : OrderAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;
    /// <summary>
    /// 门店Id
    /// </summary>
    public long StoreId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 客户单号
    /// </summary>
    public string CustomerOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 客户PO号（可空）
    /// </summary>
    public string? CustomerPONumber { get; private set; }
    /// <summary>
    /// 销售产品Id
    /// </summary>
    public long SalesSpuId { get; private set; }
    /// <summary>
    /// 销售产品Code
    /// </summary>
    public string SalesSpuCode { get; private set; } = string.Empty;
    /// <summary>
    /// 产品Id
    /// </summary>
    public long SpuId { get; private set; }
    /// <summary>
    /// 来单数量
    /// </summary>
    public int OrderQty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public int Qty { get; private set; }
    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; private set; } = string.Empty;
    /// <summary>
    /// 订单时间
    /// </summary>
    public DateTime OrderOn { get; private set; }
    /// <summary>
    /// 预出货时间（可空）
    /// </summary>
    public DateTime? DeliveryOn { get; private set; }
    /// <summary>
    /// 送仓库
    /// </summary>
    public bool SongCangKu { get; private set; }
    /// <summary>
    /// 发料
    /// </summary>
    public bool FaLiao { get; private set; }
    /// <summary>
    /// 扣账
    /// </summary>
    public bool KouZhang { get; private set; }
    /// <summary>
    /// 送包装
    /// </summary>
    public bool SongBaoZhuang { get; private set; }
    /// <summary>
    /// ETC
    /// </summary>
    public bool Etc { get; private set; }
    /// <summary>
    /// 打唛
    /// </summary>
    public bool DaMai { get; private set; }
    /// <summary>
    /// 黄油
    /// </summary>
    public bool HuangYou { get; private set; }
    /// <summary>
    /// 打印条码
    /// </summary>
    public bool DaYinTiaoMa { get; private set; }
    /// <summary>
    /// 换镜袋
    /// </summary>
    public bool HuanJingDai { get; private set; }
    /// <summary>
    /// 贴背贴
    /// </summary>
    public bool TieBeiTie { get; private set; }
    /// <summary>
    /// 打包
    /// </summary>
    public bool DaBao { get; private set; }
    /// <summary>
    /// 装箱明细
    /// </summary>
    public bool ZhuangXiangMingXi { get; private set; }
    /// <summary>
    /// 出货检查
    /// </summary>
    public bool ChuHuoJianCha { get; private set; }
    /// <summary>
    /// 封箱
    /// </summary>
    public bool FengXiang { get; private set; }
    /// <summary>
    /// 备注（可空）
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 是否打印
    /// </summary>
    public bool IsPrinted { get; private set; }
    /// <summary>
    /// 打印人Id（可空）
    /// </summary>
    public long? PrintedBy { get; private set; }
    /// <summary>
    /// 打印人（可空）
    /// </summary>
    public string? PrintedByName { get; private set; }
    /// <summary>
    /// 打印时间（可空）
    /// </summary>
    public DateTime? PrintedOn { get; private set; }
    /// <summary>
    /// 完成人Id（可空）
    /// </summary>
    public long? FinishedBy { get; private set; }
    /// <summary>
    /// 完成人名称（可空）
    /// </summary>
    public string? FinishedByName { get; private set; }
    /// <summary>
    /// 完成时间（可空）
    /// </summary>
    public DateTime? FinishedOn { get; private set; }
    /// <summary>
    /// 取消人Id（可空）
    /// </summary>
    public long? CanceledBy { get; private set; }
    /// <summary>
    /// 取消人（可空）
    /// </summary>
    public string? CanceledByName { get; private set; }
    /// <summary>
    /// 取消时间（可空）
    /// </summary>
    public DateTime? CanceledOn { get; private set; }

    /// <summary>
    /// 外部出库单跟踪列表
    /// </summary>
    private readonly List<ExternalLeaveStockOrderTracking> _externalLeaveStockOrderTrackings;

    /// <summary>
    /// 外部出库单跟踪列表
    /// </summary>
    public IReadOnlyCollection<ExternalLeaveStockOrderTracking> ExternalLeaveStockOrderTrackings
    {
        get { return _externalLeaveStockOrderTrackings.AsReadOnly(); }
    }

    /// <summary>
    /// 供 EF 使用的无参构造器
    /// </summary>
    protected ExternalLeaveStockOrder()
    {
        _externalLeaveStockOrderTrackings = new List<ExternalLeaveStockOrderTracking>();
    }

    /// <summary>
    /// 构造器
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
    internal ExternalLeaveStockOrder(
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
        string? remark)
        : this()
    {
        ProductType = productType;
        StoreId = storeId;
        CompanyId = companyId;
        CustomerOrderNumber = customerOrderNumber;
        CustomerPONumber = customerPONumber;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        SpuId = spuId;
        OrderQty = orderQty;
        Qty = qty;
        SongCangKu = songCangKu;
        FaLiao = faLiao;
        KouZhang = kouZhang;
        SongBaoZhuang = songBaoZhuang;
        Etc = etc;
        DaMai = daMai;
        HuangYou = huangYou;
        DaYinTiaoMa = daYinTiaoMa;
        HuanJingDai = huanJingDai;
        TieBeiTie = tieBeiTie;
        DaBao = daBao;
        ZhuangXiangMingXi = zhuangXiangMingXi;
        ChuHuoJianCha = chuHuoJianCha;
        FengXiang = fengXiang;
        OrderOn = orderOn;
        DeliveryOn = deliveryOn;
        Remark = remark;
        Status = nameof(ExternalLeaveStockOrderStatusEnum.Pending);
        AddDomainEvent(new ExternalLeaveStockOrderAddedOrModifiedEvent(this));
    }

    /// <summary>
    /// 修改
    /// </summary>
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
    internal void Modify(
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
        string? remark)
    {
        if (Status != nameof(ExternalLeaveStockOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("外部出库单" + Number + "当前状态不允许修改!");
        }

        CustomerOrderNumber = customerOrderNumber;
        CustomerPONumber = customerPONumber;
        SalesSpuId = salesSpuId;
        SalesSpuCode = salesSpuCode;
        SpuId = spuId;
        OrderQty = orderQty;
        Qty = qty;
        SongCangKu = songCangKu;
        FaLiao = faLiao;
        KouZhang = kouZhang;
        SongBaoZhuang = songBaoZhuang;
        Etc = etc;
        DaMai = daMai;
        HuangYou = huangYou;
        DaYinTiaoMa = daYinTiaoMa;
        HuanJingDai = huanJingDai;
        TieBeiTie = tieBeiTie;
        DaBao = daBao;
        ZhuangXiangMingXi = zhuangXiangMingXi;
        ChuHuoJianCha = chuHuoJianCha;
        FengXiang = fengXiang;
        OrderOn = orderOn;
        DeliveryOn = deliveryOn;
        Remark = remark;
        AddDomainEvent(new ExternalLeaveStockOrderAddedOrModifiedEvent(this));
        Audit(nameof(AuditActionEnum.Modify));
    }

    /// <summary>
    /// 添加跟踪（按工序）
    /// </summary>
    /// <param name="workStepId">工序Id</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal void AddTracking(long workStepId, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        if (Status == nameof(ExternalLeaveStockOrderStatusEnum.Canceled))
        {
            throw new UserFriendlyException("外部出库单" + Number + "已取消,不允许录入!");
        }

        _externalLeaveStockOrderTrackings.Add(new ExternalLeaveStockOrderTracking(workStepId, employeeId, outDepartmentId, inDepartmentId));
    }

    /// <summary>
    /// 添加跟踪（按原因）
    /// </summary>
    /// <param name="reason">原因（可空）</param>
    /// <param name="employeeId">员工Id</param>
    /// <param name="outDepartmentId">出部门Id</param>
    /// <param name="inDepartmentId">入部门Id</param>
    internal void AddTracking(string? reason, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        _externalLeaveStockOrderTrackings.Add(new ExternalLeaveStockOrderTracking(reason, employeeId, outDepartmentId, inDepartmentId));
    }

    /// <summary>
    /// 完成
    /// </summary>
    /// <param name="userId">完成人Id</param>
    /// <param name="name">完成人名称</param>
    public void Finish(long userId, string name)
    {
        if (Status != nameof(ExternalLeaveStockOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("外部出库单" + Number + "当前状态不允许完成!");
        }

        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        Status = nameof(ExternalLeaveStockOrderStatusEnum.Finished);
        Audit(nameof(AuditActionEnum.Finish));
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注（可空）</param>
    public void Cancel(long userId, string name, string? remark)
    {
        if (Status != nameof(ExternalLeaveStockOrderStatusEnum.Pending))
        {
            throw new UserFriendlyException("外部出库单" + Number + "当前状态不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = nameof(ExternalLeaveStockOrderStatusEnum.Canceled);
        AddDomainEvent(new ExternalLeaveStockOrderCanceledEvent(this));
        Audit(nameof(AuditActionEnum.Cancel), remark);
    }

    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="userId">打印人Id</param>
    /// <param name="name">打印人名称</param>
    public void Print(long userId, string name)
    {
        IsPrinted = true;
        PrintedBy = userId;
        PrintedByName = name;
        PrintedOn = DateTime.Now;
        Audit(nameof(AuditActionEnum.Print));
    }

    /// <inheritdoc />
    public override string GetSerialNumberType()
    {
        if (ProductType == nameof(ProductTypeEnum.SemiProduct))
        {
            return nameof(SerialNumberTypeEnum.SemiProductExternalLeaveStockOrder);
        }

        return nameof(SerialNumberTypeEnum.EndProductExternalLeaveStockOrder);
    }
}
