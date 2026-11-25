using MXERP.Domain.Common;
using Snow.Exceptions;

namespace MXERP.Domain.ExternalOrderLogs;

/// <summary>
/// 外部订单日志
/// </summary>
public class ExternalOrderLog : LogAggregateRoot
{
    /// <summary>
    /// TraceId（可空）
    /// </summary>
    public string? TraceId { get; private set; }
    /// <summary>
    /// 类型
    /// </summary>
    public ExternalOrderLogTypeEnum Type { get; private set; }
    /// <summary>
    /// 流水號HK（可空）
    /// </summary>
    public string? HKNumber { get; private set; }
    /// <summary>
    /// 跟单条码（可空）
    /// </summary>
    public string? DocumentaryBarCode { get; private set; }
    /// <summary>
    /// 客户单号
    /// </summary>
    public string CustomerOrderNumber { get; private set; } = string.Empty;
    /// <summary>
    /// 客户PO号（可空）
    /// </summary>
    public string? CustomerPONumber { get; private set; }
    /// <summary>
    /// 公司订单编号（可空）
    /// </summary>
    public string? OrderRef { get; private set; }
    /// <summary>
    /// 客户流水号（可空）
    /// </summary>
    public string? CustomerSerialNumber { get; private set; }
    /// <summary>
    /// 接口地址
    /// </summary>
    public string ApiUrl { get; private set; } = string.Empty;
    /// <summary>
    /// 接口提交方法
    /// </summary>
    public string ApiMethod { get; private set; } = string.Empty;
    /// <summary>
    /// IP地址（可空）
    /// </summary>
    public string? Ip { get; private set; }
    /// <summary>
    /// 客户端地点（可空）
    /// </summary>
    public string? City { get; private set; }
    /// <summary>
    /// 客户端UserAgent（可空）
    /// </summary>
    public string? UserAgent { get; private set; }
    /// <summary>
    /// 浏览器（可空）
    /// </summary>
    public string? Browser { get; private set; }
    /// <summary>
    /// 操作系统（可空）
    /// </summary>
    public string? Os { get; private set; }

    /// <summary>
    /// 构造器（供 EF 使用）
    /// </summary>
    protected ExternalOrderLog()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="traceId">TraceId（可空）</param>
    /// <param name="type">类型</param>
    /// <param name="userId">用户Id（可空）</param>
    /// <param name="userName">用户名称（可空）</param>
    /// <param name="startedOn">开始时间</param>
    /// <param name="endedOn">结束时间</param>
    /// <param name="elapsedMilliseconds">耗时（毫秒）</param>
    /// <param name="hkNumber">流水號HK（可空）</param>
    /// <param name="documentaryBarCode">跟单条码（可空）</param>
    /// <param name="customerOrderNumber">客户单号</param>
    /// <param name="customerPONumber">客户PO号（可空）</param>
    /// <param name="orderRef">公司订单编号（可空）</param>
    /// <param name="customerSerialNumber">客户流水号（可空）</param>
    /// <param name="apiUrl">接口地址</param>
    /// <param name="apiMethod">接口提交方法</param>
    /// <param name="ip">IP地址（可空）</param>
    /// <param name="city">客户端地点（可空）</param>
    /// <param name="userAgent">客户端UserAgent（可空）</param>
    /// <param name="browser">浏览器（可空）</param>
    /// <param name="os">操作系统（可空）</param>
    /// <param name="request">请求数据（可空）</param>
    /// <param name="response">返回数据（可空）</param>
    /// <param name="isSuccess">是否成功</param>
    /// <param name="errorMessage">错误信息（可空）</param>
    /// <param name="exception">异常信息（可空）</param>
    public ExternalOrderLog(string? traceId, ExternalOrderLogTypeEnum type, long? userId, string? userName, DateTime startedOn, DateTime endedOn, long elapsedMilliseconds, string? hkNumber, string? documentaryBarCode, string customerOrderNumber, string? customerPONumber, string? orderRef, string? customerSerialNumber, string apiUrl, string apiMethod, string? ip, string? city, string? userAgent, string? browser, string? os, string? request, string? response, bool isSuccess, string? errorMessage, string? exception)
        : base(userId, userName, startedOn, endedOn, elapsedMilliseconds, request, response, isSuccess, errorMessage, exception)
    {
        TraceId = traceId;
        Type = type;
        HKNumber = hkNumber;
        DocumentaryBarCode = documentaryBarCode;
        CustomerOrderNumber = customerOrderNumber;
        CustomerPONumber = customerPONumber;
        OrderRef = orderRef;
        CustomerSerialNumber = customerSerialNumber;
        ApiUrl = apiUrl;
        ApiMethod = apiMethod;
        Ip = ip;
        City = city;
        UserAgent = userAgent;
        Browser = browser;
        Os = os;
    }

}
