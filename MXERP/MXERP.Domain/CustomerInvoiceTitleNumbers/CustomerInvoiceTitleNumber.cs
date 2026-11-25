using MXERP.Domain.Common.Enums;
using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.CustomerInvoiceTitleNumbers;

/// <summary>
/// 客户票号表
/// </summary>
public class CustomerInvoiceTitleNumber : AuditedAggregateRoot
{
    /// <summary>
    /// 客户代码
    /// </summary>
    public string InvoiceTitle { get; private set; } = string.Empty;

    /// <summary>
    /// 号码
    /// </summary>
    public int Number { get; private set; }

    /// <summary>
    /// 年
    /// </summary>
    public int Year { get; private set; }

    /// <summary>
    /// 月
    /// </summary>
    public int Month { get; private set; }

    /// <summary>
    /// 日
    /// </summary>
    public int Day { get; private set; }

    /// <summary>
    /// 补充数字
    /// </summary>
    public int PadNumber { get; private set; }

    /// <summary>
    /// 规则(可空)
    /// </summary>
    public DateTypeEnum? Rule { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected CustomerInvoiceTitleNumber()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="invoiceTitle">发票抬头</param>
    /// <param name="customerInvoiceTitlePadNumber">补充数字</param>
    internal CustomerInvoiceTitleNumber(string invoiceTitle, int customerInvoiceTitlePadNumber)
    {
        InvoiceTitle = (invoiceTitle ?? string.Empty).Trim();
        Number = 1;

        DateTime now = DateTime.Now;
        Year = now.Year;
        Month = now.Month;
        Day = now.Day;

        PadNumber = customerInvoiceTitlePadNumber;
        Rule = DateTypeEnum.Month;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="padNumber">补充数字</param>
    /// <param name="rule">规则(可空)</param>
    public void Modify(int padNumber, DateTypeEnum? rule)
    {
        PadNumber = padNumber;
        if (rule is not null)
        {
            Rule = rule;
        }
    }

    /// <summary>
    /// 流水号自增
    /// </summary>
    public void Increment()
    {
        DateTime now = DateTime.Now;
        int number = Number;
        int nowYear = now.Year;
        int nowMonth = now.Month;
        int nowDay = now.Day;

        if (Year != nowYear)
        {
            Year = nowYear;
            if (DateTypeEnum.Year == Rule)
            {
                number = 1;
            }
        }

        if (Month != nowMonth)
        {
            Month = nowMonth;
            if (DateTypeEnum.Month == Rule)
            {
                number = 1;
            }
        }

        if (Day != nowDay)
        {
            Day = nowDay;
            if (DateTypeEnum.Day == Rule)
            {
                number = 1;
            }
        }

        Number = number + 1;
    }

    /// <summary>
    /// 生成流水号
    /// </summary>
    /// <returns>流水号</returns>
    public string GenerateNumber()
    {
        DateTime now = DateTime.Now;
        int number = Number;
        int nowYear = now.Year;
        int nowMonth = now.Month;
        int nowDay = now.Day;

        if (Year != nowYear && DateTypeEnum.Year == Rule)
        {
            number = 1;
        }

        if (Month != nowMonth && DateTypeEnum.Month == Rule)
        {
            number = 1;
        }

        if (Day != nowDay && DateTypeEnum.Day == Rule)
        {
            number = 1;
        }

        string monthPart = now.ToString("MM");
        return InvoiceTitle + "-" + monthPart + "-" + (number + PadNumber);
    }
}
