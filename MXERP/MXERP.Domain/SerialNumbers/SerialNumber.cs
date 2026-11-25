using MXERP.Domain.Common.Enums;
using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.SerialNumbers;

/// <summary>
/// 流水号表
/// </summary>
public class SerialNumber : AuditedAggregateRoot
{
    /// <summary>
    /// 类型
    /// </summary>
    public SerialNumberTypeEnum Type { get; private set; }

    /// <summary>
    /// 号码
    /// </summary>
    public int Number { get; private set; }

    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix { get; private set; } = string.Empty;

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
    /// 时间格式(可空)
    /// </summary>
    public string? DateFormat { get; private set; }

    /// <summary>
    /// 补充数量
    /// </summary>
    public int Pad { get; private set; }

    /// <summary>
    /// 补充符号
    /// </summary>
    public string PadSymbol { get; private set; } = string.Empty;

    /// <summary>
    /// 是否分割
    /// </summary>
    public bool IsSplit { get; private set; }

    /// <summary>
    /// 分割符号
    /// </summary>
    public string SplitSymbol { get; private set; } = string.Empty;

    /// <summary>
    /// 规则(可空)
    /// </summary>
    public DateTypeEnum? Rule { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected SerialNumber()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="prefix">前缀</param>
    /// <param name="dateFormat">时间格式(可空)</param>
    /// <param name="pad">补充数量</param>
    /// <param name="padSymbol">补充符号</param>
    /// <param name="isSplit">是否分割</param>
    /// <param name="splitSymbol">分割符号</param>
    /// <param name="rule">规则(可空)</param>
    internal SerialNumber(SerialNumberTypeEnum type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, DateTypeEnum? rule)
    {
        Type = type;
        Number = 1;
        Prefix = prefix;

        DateTime now = DateTime.Now;
        Year = now.Year;
        Month = now.Month;
        Day = now.Day;

        DateFormat = dateFormat;
        Pad = pad;
        PadSymbol = padSymbol;
        IsSplit = isSplit;
        SplitSymbol = splitSymbol;
        Rule = rule;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="type">类型</param>
    /// <param name="prefix">前缀</param>
    /// <param name="dateFormat">时间格式(可空)</param>
    /// <param name="pad">补充数量</param>
    /// <param name="padSymbol">补充符号</param>
    /// <param name="isSplit">是否分割</param>
    /// <param name="splitSymbol">分割符号</param>
    /// <param name="rule">规则(可空)</param>
    public void Modify(SerialNumberTypeEnum type, string prefix, string? dateFormat, int pad, string padSymbol, bool isSplit, string splitSymbol, DateTypeEnum? rule)
    {
        Type = type;
        Prefix = prefix;
        DateFormat = dateFormat;
        Pad = pad;
        PadSymbol = padSymbol;
        IsSplit = isSplit;
        SplitSymbol = splitSymbol;
        Rule = rule;
    }

    /// <summary>
    /// 生成流水号
    /// </summary>
    /// <returns>流水号</returns>
    public string GetNextNumber()
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

        string split = string.Empty;
        if (IsSplit)
        {
            split = "-";
        }

        string datePart = string.Empty;
        if (!string.IsNullOrWhiteSpace(DateFormat))
        {
            datePart = now.ToString(DateFormat) + split;
        }

        char padChar = '0';
        if (PadSymbol.Length > 0)
        {
            padChar = PadSymbol[0];
        }

        return Prefix + split + datePart + Number.ToString().PadLeft(Pad, padChar);
    }

}
