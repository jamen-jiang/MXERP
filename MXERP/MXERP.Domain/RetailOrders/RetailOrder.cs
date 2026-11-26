using MXERP.Domain.Common.Enums;
using MXERP.Domain.AuditLogs;
using MXERP.Domain.Common.Aggregates;
using MXERP.Domain.SerialNumbers;
using Snow.Exceptions;

namespace MXERP.Domain.RetailOrders;

/// <summary>
/// 散镜订单(对应 Java RetailOrder)
/// </summary>
public class RetailOrder : OrderAggregateRoot
{
    /// <summary>
    /// 传出区(可空)
    /// </summary>
    public string? OutArea { get; private set; }
    /// <summary>
    /// 流水号(可空)
    /// </summary>
    public string? SequenceNumber { get; private set; }
    /// <summary>
    /// 流水号HK(可空)
    /// </summary>
    public string? HkNumber { get; private set; }
    /// <summary>
    /// 客户单号
    /// </summary>
    public string CustomerOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 客户PO号(可空)
    /// </summary>
    public string? CustomerPONumber { get; private set; }
    /// <summary>
    /// 跟单条码(可空)
    /// </summary>
    public string? DocumentaryBarCode { get; private set; }
    /// <summary>
    /// 门店Id
    /// </summary>
    public long StoreId { get; private set; }
    /// <summary>
    /// 公司Id
    /// </summary>
    public long CompanyId { get; private set; }
    /// <summary>
    /// 拟出货
    /// </summary>
    public int OutGood { get; private set; }
    /// <summary>
    /// 拟出时段(可空)
    /// </summary>
    public string? OutGoodTime { get; private set; }
    /// <summary>
    /// 玻璃胶片(可空)
    /// </summary>
    public string? Material { get; private set; }
    /// <summary>
    /// 数量
    /// </summary>
    public double Qty { get; private set; }
    /// <summary>
    /// 出库数量
    /// </summary>
    public double LeaveStockQty { get; private set; }
    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; private set; } = string.Empty;
    /// <summary>
    /// 发票打印时间(可空)
    /// </summary>
    public DateTime? InvoicePrintOn { get; private set; }
    /// <summary>
    /// 是否打印捡料单
    /// </summary>
    public bool IsPrinted { get; private set; }
    /// <summary>
    /// 备注(可空)
    /// </summary>
    public string? Remark { get; private set; }
    /// <summary>
    /// 打印人Id(可空)
    /// </summary>
    public long? PrintedBy { get; private set; }
    /// <summary>
    /// 打印人(可空)
    /// </summary>
    public string? PrintedByName { get; private set; }
    /// <summary>
    /// 打印时间(可空)
    /// </summary>
    public DateTime? PrintedOn { get; private set; }
    /// <summary>
    /// 审核人Id(可空)
    /// </summary>
    public long? CheckedBy { get; private set; }
    /// <summary>
    /// 审核人(可空)
    /// </summary>
    public string? CheckedByName { get; private set; }
    /// <summary>
    /// 审核时间(可空)
    /// </summary>
    public DateTime? CheckedOn { get; private set; }
    /// <summary>
    /// 完成人Id(可空)
    /// </summary>
    public long? FinishedBy { get; private set; }
    /// <summary>
    /// 完成人名称(可空)
    /// </summary>
    public string? FinishedByName { get; private set; }
    /// <summary>
    /// 完成时间(可空)
    /// </summary>
    public DateTime? FinishedOn { get; private set; }
    /// <summary>
    /// 取消人Id(可空)
    /// </summary>
    public long? CanceledBy { get; private set; }
    /// <summary>
    /// 取消人(可空)
    /// </summary>
    public string? CanceledByName { get; private set; }
    /// <summary>
    /// 取消时间(可空)
    /// </summary>
    public DateTime? CanceledOn { get; private set; }
    /// <summary>
    /// 是否同步到HkoTracking
    /// </summary>
    public bool IsSyncHkoTracking { get; private set; }
    /// <summary>
    /// 工序
    /// </summary>
    public ProcessVO Process { get; private set; } = null!;

    /// <summary>
    /// 散镜订单项
    /// </summary>
    private readonly List<RetailOrderItem> _retailOrderItems;

    /// <summary>
    /// 散镜订单项
    /// </summary>
    public IReadOnlyCollection<RetailOrderItem> RetailOrderItems
    {
        get { return _retailOrderItems.AsReadOnly(); }
    }

    /// <summary>
    /// 散镜订单跟踪记录
    /// </summary>
    private readonly List<RetailOrderTracking> _retailOrderTrackings;

    /// <summary>
    /// 散镜订单跟踪记录
    /// </summary>
    public IReadOnlyCollection<RetailOrderTracking> RetailOrderTrackings
    {
        get { return _retailOrderTrackings.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected RetailOrder()
    {
        _retailOrderItems = new List<RetailOrderItem>();
        _retailOrderTrackings = new List<RetailOrderTracking>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="storeId">门店Id</param>
    /// <param name="companyId">公司Id</param>
    /// <param name="outArea">传出区(可空)</param>
    /// <param name="sequenceNumber">流水号(可空)</param>
    /// <param name="hkNumber">流水号HK(可空)</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号(可空)</param>
    /// <param name="documentaryBarCode">跟单条码(可空)</param>
    /// <param name="outGood">拟出货</param>
    /// <param name="outGoodTime">拟出时段(可空)</param>
    /// <param name="material">玻璃胶片(可空)</param>
    /// <param name="coating">焗色(可空)</param>
    /// <param name="tintingColor">茶色(可空)</param>
    /// <param name="mirror">水银(可空)</param>
    /// <param name="tintingCode">染色(可空)</param>
    /// <param name="tintingName">染色名称(可空)</param>
    /// <param name="uv">UV(可空)</param>
    /// <param name="hard">加硬(可空)</param>
    /// <param name="polish">抛光(可空)</param>
    /// <param name="auro">彩边(可空)</param>
    /// <param name="edge">裁型/车边(可空)</param>
    /// <param name="superHole">开坑(可空)</param>
    /// <param name="special">批花(可空)</param>
    /// <param name="specialCode">批花代号(可空)</param>
    /// <param name="zuanKong">钻孔(可空)</param>
    /// <param name="frame">镜架(可空)</param>
    /// <param name="remark">备注(可空)</param>
    internal RetailOrder(long storeId, long companyId, string? outArea, string? sequenceNumber, string? hkNumber, string customerOrderNumber, string? customerPONumber, string? documentaryBarCode, int outGood, string? outGoodTime, string? material, string? coating, string? tintingColor, string? mirror, string? tintingCode, string? tintingName, bool? uv, bool? hard, bool? polish, string? auro, string? edge, string? superHole, string? special, string? specialCode, string? zuanKong, string? frame, string? remark)
        : this()
    {
        StoreId = storeId;
        CompanyId = companyId;
        OutArea = outArea;
        SequenceNumber = sequenceNumber;
        HkNumber = hkNumber;
        CustomerOrderNumber = customerOrderNumber;
        CustomerPONumber = TrimToNull(customerPONumber);
        DocumentaryBarCode = documentaryBarCode;
        OutGood = outGood;
        OutGoodTime = outGoodTime;
        Material = material;
        Remark = remark;
        Process = new ProcessVO(coating, tintingColor, mirror, tintingCode, tintingName, uv ?? false, hard ?? false, polish ?? false, auro, edge, superHole, special, specialCode, zuanKong, frame);
        Status = nameof(RetailOrderStatusEnum.Pending);
        AddDomainEvent(new RetailOrderAddedEvent(this));
    }

    /// <summary>
    /// 修改
    /// </summary>
    public void Modify(string? outArea, string? sequenceNumber, string? hkNumber, string customerOrderNumber, string? customerPONumber, string? documentaryBarCode, int outGood, string? outGoodTime, string? material, string? coating, string? tintingColor, string? mirror, string? tintingCode, string? tintingName, bool? uv, bool? hard, bool? polish, string? auro, string? edge, string? superHole, string? special, string? specialCode, string? zuanKong, string? frame, string? remark)
    {
        if (nameof(RetailOrderStatusEnum.Pending) != Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "当前状态不允许修改!");
        }

        OutArea = outArea;
        SequenceNumber = sequenceNumber;
        HkNumber = hkNumber;
        CustomerOrderNumber = customerOrderNumber;
        CustomerPONumber = TrimToNull(customerPONumber);
        DocumentaryBarCode = documentaryBarCode;
        OutGood = outGood;
        OutGoodTime = outGoodTime;
        Material = material;
        Remark = remark;
        Process = new ProcessVO(coating, tintingColor, mirror, tintingCode, tintingName, uv ?? false, hard ?? false, polish ?? false, auro, edge, superHole, special, specialCode, zuanKong, frame);
        Audit(nameof(AuditActionEnum.Modify));
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

    /// <summary>
    /// 审核
    /// </summary>
    /// <param name="userId">审核人Id</param>
    /// <param name="name">审核人名称</param>
    public void Check(long userId, string name)
    {
        if (nameof(RetailOrderStatusEnum.Pending) != Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "当前状态不允许审核!");
        }

        CheckedBy = userId;
        CheckedByName = name;
        CheckedOn = DateTime.Now;
        Status = nameof(RetailOrderStatusEnum.Delivery);
        AddDomainEvent(new RetailOrderCheckedEvent(this));
        Audit(nameof(AuditActionEnum.Check));
    }

    /// <summary>
    /// 弃审
    /// </summary>
    /// <param name="remark">备注(可空)</param>
    internal void UnCheck(string? remark)
    {
        if (nameof(RetailOrderStatusEnum.Delivery) != Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "当前状态不允许弃审!");
        }

        CheckedBy = null;
        CheckedByName = null;
        CheckedOn = null;
        Status = nameof(RetailOrderStatusEnum.Pending);
        Audit(nameof(AuditActionEnum.UnCheck), remark);
    }

    /// <summary>
    /// 取消
    /// </summary>
    /// <param name="userId">取消人Id</param>
    /// <param name="name">取消人名称</param>
    /// <param name="remark">备注(可空)</param>
    internal void Cancel(long userId, string name, string? remark)
    {
        if (nameof(RetailOrderStatusEnum.Pending) != Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "当前状态不允许取消!");
        }

        CanceledBy = userId;
        CanceledByName = name;
        CanceledOn = DateTime.Now;
        Status = nameof(RetailOrderStatusEnum.Canceled);
        Audit(nameof(AuditActionEnum.Cancel), remark);
    }

    /// <summary>
    /// 完成
    /// </summary>
    /// <param name="userId">完成人Id</param>
    /// <param name="name">完成人名称</param>
    internal void Finish(long userId, string name)
    {
        if (nameof(RetailOrderStatusEnum.Delivered) != Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "当前状态不允许完成!");
        }

        FinishedBy = userId;
        FinishedByName = name;
        FinishedOn = DateTime.Now;
        Status = nameof(RetailOrderStatusEnum.Finished);
        AddDomainEvent(new RetailOrderFinishedEvent(this));
        Audit(nameof(AuditActionEnum.Finish));
    }

    /// <summary>
    /// 弃完
    /// </summary>
    /// <param name="remark">备注(可空)</param>
    internal void UnFinish(string? remark)
    {
        if (nameof(RetailOrderStatusEnum.Finished) != Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "当前状态不允许退回!");
        }

        FinishedBy = null;
        FinishedByName = null;
        FinishedOn = null;
        Status = nameof(RetailOrderStatusEnum.Delivered);
        Audit(nameof(AuditActionEnum.UnFinish), remark);
    }

    /// <summary>
    /// 添加或修改项
    /// </summary>
    public void AddOrModifyItem(string eyeType, long salesSpuId, string salesSpuCode, long? customerSpuId, string? customerSpuCode, long spuId, long warehouseId, int sph, int cyl, int? add, int? diameter, int? axis, double? pd, double? ph, string? baseCurve, double? centerThickness, bool? knifeSide, string? decenter, string? prism, double qty)
    {
        RetailOrderItem? retailOrderItem = GetItem(eyeType);
        if (retailOrderItem is not null)
        {
            retailOrderItem.Modify(eyeType, salesSpuId, salesSpuCode, customerSpuId, customerSpuCode, spuId, warehouseId, sph, cyl, add, diameter, axis, pd, ph, baseCurve, centerThickness, knifeSide, decenter, prism, qty);
        }
        else
        {
            retailOrderItem = new RetailOrderItem(eyeType, salesSpuId, salesSpuCode, customerSpuId, customerSpuCode, spuId, warehouseId, sph, cyl, add, diameter, axis, pd, ph, baseCurve, centerThickness, knifeSide, decenter, prism, qty);
            _retailOrderItems.Add(retailOrderItem);
        }

        UpdateQty();
    }

    /// <summary>
    /// 更新项出库数量(正数=添加, 负数=减去)
    /// </summary>
    /// <param name="retailOrderItemId">散镜订单项Id</param>
    /// <param name="leaveStockQty">出库数量</param>
    public void UpdateLeaveStockQty(long retailOrderItemId, double leaveStockQty)
    {
        RetailOrderItem? retailOrderItem = GetItem(retailOrderItemId);
        if (retailOrderItem is not null)
        {
            retailOrderItem.UpdateLeaveStockQty(leaveStockQty);
            UpdateLeaveStockQty();
        }
    }

    /// <summary>
    /// 添加跟踪(按工序)
    /// </summary>
    internal void AddTracking(long workStepId, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        if (nameof(RetailOrderStatusEnum.Canceled) == Status)
        {
            throw new UserFriendlyException("散镜订单" + Number + "已取消,不允许录入!");
        }

        _retailOrderTrackings.Add(new RetailOrderTracking(workStepId, employeeId, outDepartmentId, inDepartmentId));
    }

    /// <summary>
    /// 添加跟踪(按原因)
    /// </summary>
    internal void AddTracking(string? reason, long employeeId, long outDepartmentId, long inDepartmentId)
    {
        _retailOrderTrackings.Add(new RetailOrderTracking(reason, employeeId, outDepartmentId, inDepartmentId));
    }

    /// <summary>
    /// 同步 HKO 跟踪(订单级)
    /// </summary>
    public void SyncHkoTracking()
    {
        IsSyncHkoTracking = true;
    }

    /// <summary>
    /// 同步 HKO 跟踪(跟踪项)
    /// </summary>
    /// <param name="retailOrderTrackingId">散镜订单跟踪Id</param>
    public void SyncHkoTracking(long retailOrderTrackingId)
    {
        RetailOrderTracking? retailOrderTracking = _retailOrderTrackings.FirstOrDefault(x => x.Id == retailOrderTrackingId);
        if (retailOrderTracking is not null)
        {
            retailOrderTracking.SyncHkoTracking();
        }
    }

    /// <summary>
    /// 序列号类型
    /// </summary>
    /// <returns>序列号类型</returns>
    public override string GetSerialNumberType()
    {
        return nameof(SerialNumberTypeEnum.RetailOrder);
    }

    private RetailOrderItem? GetItem(string eyeType)
    {
        return _retailOrderItems.FirstOrDefault(x => x.EyeType == eyeType);
    }

    private RetailOrderItem? GetItem(long retailOrderItemId)
    {
        return _retailOrderItems.FirstOrDefault(x => x.Id == retailOrderItemId);
    }

    private void UpdateQty()
    {
        Qty = _retailOrderItems.Where(x => !x.IsDeleted).Sum(x => x.Qty);
    }

    private void UpdateLeaveStockQty()
    {
        LeaveStockQty = _retailOrderItems.Where(x => !x.IsDeleted).Sum(x => x.LeaveStockQty);
        if (Qty - LeaveStockQty > 0)
        {
            Status = nameof(RetailOrderStatusEnum.Delivery);
        }
        else
        {
            Status = nameof(RetailOrderStatusEnum.Delivered);
        }
    }

    private static string? TrimToNull(string? value)
    {
        if (value is null)
        {
            return null;
        }

        string t = value.Trim();
        if (t.Length == 0)
        {
            return null;
        }

        return t;
    }
}
