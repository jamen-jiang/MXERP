using Snow.Domain.Entities.Auditing;

namespace MXERP.Domain.Emails;

/// <summary>
/// 邮件附件
/// </summary>
public class EmailAttachment : DeletionAuditedEntity
{
    /// <summary>
    /// 邮件Id
    /// </summary>
    public long? EmailId { get; private set; }

    /// <summary>
    /// 附件名
    /// </summary>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>
    /// 附件
    /// </summary>
    public byte[]? Data { get; private set; }

    /// <summary>
    /// 构造器
    /// </summary>
    protected EmailAttachment()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="fileName">附件名</param>
    /// <param name="data">附件</param>
    internal EmailAttachment(string fileName, byte[]? data)
    {
        FileName = fileName;
        Data = data;
    }

    /// <summary>
    /// 修改附件
    /// </summary>
    /// <param name="data">附件</param>
    internal void Modify(byte[]? data)
    {
        Data = data;
    }
}
