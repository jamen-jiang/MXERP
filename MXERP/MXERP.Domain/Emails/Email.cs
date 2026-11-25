using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Emails;

/// <summary>
/// 邮件
/// </summary>
public class Email : AuditedAggregateRoot
{
    /// <summary>
    /// 发送人邮箱
    /// </summary>
    public string From { get; private set; } = string.Empty;

    /// <summary>
    /// 发送人名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 收件人
    /// </summary>
    public string To { get; private set; } = string.Empty;

    /// <summary>
    /// 抄送人
    /// </summary>
    public string? Cc { get; private set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? ExceptionText { get; private set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    public DateTime? SendedOn { get; private set; }

    /// <summary>
    /// 附件列表
    /// </summary>
    private readonly List<EmailAttachment> _emailAttachments;

    /// <summary>
    /// 附件列表
    /// </summary>
    public IReadOnlyCollection<EmailAttachment> EmailAttachments
    {
        get { return _emailAttachments.AsReadOnly(); }
    }

    /// <summary>
    /// 构造器
    /// </summary>
    protected Email()
    {
        _emailAttachments = new List<EmailAttachment>();
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="from">发送人邮箱</param>
    /// <param name="name">发送人名称</param>
    /// <param name="to">收件人</param>
    /// <param name="cc">抄送人</param>
    /// <param name="subject">标题</param>
    /// <param name="content">内容</param>
    /// <param name="exception">异常</param>
    public Email(string from, string name, string to, string? cc, string subject, string content, System.Exception? exception)
        : this()
    {
        From = from;
        Name = name;
        To = to;
        Cc = cc;
        Subject = subject;
        Content = content;
        IsSuccess = exception is null;
        ErrorMessage = exception?.Message;
        ExceptionText = exception?.ToString();
        SendedOn = DateTime.Now;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="from">发送人邮箱</param>
    /// <param name="name">发送人名称</param>
    /// <param name="to">收件人</param>
    /// <param name="cc">抄送人</param>
    /// <param name="subject">标题</param>
    /// <param name="content">内容</param>
    public void Modify(string from, string name, string to, string? cc, string subject, string content)
    {
        From = from;
        Name = name;
        To = to;
        Cc = cc;
        Subject = subject;
        Content = content;
    }

    /// <summary>
    /// 修改发送结果
    /// </summary>
    /// <param name="exception">异常</param>
    public void Modify(System.Exception? exception)
    {
        IsSuccess = exception is null;
        ErrorMessage = exception?.Message;
        ExceptionText = exception?.ToString();
        SendedOn = DateTime.Now;
    }

    /// <summary>
    /// 添加或修改附件
    /// </summary>
    /// <param name="id">附件Id</param>
    /// <param name="fileName">附件名</param>
    /// <param name="data">附件</param>
    /// <param name="isDeleted">是否删除</param>
    public void AddOrModifyAttachment(long? id, string fileName, byte[]? data, bool isDeleted)
    {
        EmailAttachment? attachment = _emailAttachments.FirstOrDefault(a => a.Id == id);
        if (isDeleted)
        {
            attachment?.Delete();
            return;
        }

        if (_emailAttachments.Any(a => a.FileName == fileName && a.Id != id))
        {
            throw new UserFriendlyException($"邮件已存在对应附件{fileName}!");
        }

        if (attachment is null)
        {
            _emailAttachments.Add(new EmailAttachment(fileName, data));
        }
        else
        {
            attachment.Modify(data);
        }
    }

    /// <summary>
    /// 获取附件列表(过滤已删除)
    /// </summary>
    /// <returns>附件列表</returns>
    public IReadOnlyList<EmailAttachment> GetEmailAttachments()
    {
        return _emailAttachments.Where(a => !a.IsDeleted).ToList();
    }

}
