using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Attachments;

/// <summary>
/// 附件
/// </summary>
public class Attachment : AuditedAggregateRoot
{
    /// <summary>
    /// 持有者Id(可空)
    /// </summary>
    public long? OwnerId { get; private set; }

    /// <summary>
    /// 持有者类型
    /// </summary>
    public string OwnerType { get; private set; } = string.Empty;

    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; private set; }

    /// <summary>
    /// 文件后缀名
    /// </summary>
    public string FileExtend { get; private set; } = string.Empty;

    /// <summary>
    /// 绝对路径
    /// </summary>
    public string BasePath { get; private set; } = string.Empty;

    /// <summary>
    /// 保存路径
    /// </summary>
    public string SavePath { get; private set; } = string.Empty;

    /// <summary>
    /// 无参构造器(供 EF 使用)
    /// </summary>
    protected Attachment()
    {
    }

    /// <summary>
    /// 构造器(自动生成文件名与保存路径)
    /// </summary>
    /// <param name="ownerId">持有者Id(可空)</param>
    /// <param name="ownerType">持有者类型</param>
    /// <param name="fileExtend">文件后缀名</param>
    /// <param name="fileSize">文件大小</param>
    public Attachment(long? ownerId, string ownerType, string fileExtend, long fileSize)
    {
        OwnerId = ownerId;
        OwnerType = ValidateOwnerType(ownerType);
        FileExtend = (fileExtend ?? string.Empty).Replace(".", string.Empty).Trim();
        FileSize = fileSize;

        string datePart = DateTime.Now.ToString("yyyyMMdd");
        string filePath = "/" + datePart + "/";
        string fileName = Guid.NewGuid().ToString() + "." + FileExtend;
        FileName = fileName;
        SavePath = filePath + fileName;
    }

    /// <summary>
    /// 构造器(指定文件名与路径)
    /// </summary>
    /// <param name="ownerId">持有者Id(可空)</param>
    /// <param name="ownerType">持有者类型</param>
    /// <param name="fileName">文件名称</param>
    /// <param name="fileExtend">文件后缀名</param>
    /// <param name="fileSize">文件大小</param>
    /// <param name="savePath">保存路径</param>
    /// <param name="basePath">绝对路径</param>
    public Attachment(long? ownerId, string ownerType, string fileName, string fileExtend, long fileSize, string savePath, string basePath)
    {
        OwnerId = ownerId;
        OwnerType = ValidateOwnerType(ownerType);
        FileName = (fileName ?? string.Empty).Trim();
        FileExtend = (fileExtend ?? string.Empty).Replace(".", string.Empty).Trim();
        FileSize = fileSize;
        SavePath = (savePath ?? string.Empty).Trim();
        BasePath = (basePath ?? string.Empty).Trim();
    }

    /// <summary>
    /// 修改持有者Id
    /// </summary>
    /// <param name="ownerId">持有者Id</param>
    public void ChangeOwnerId(long ownerId)
    {
        OwnerId = ownerId;
    }

    /// <summary>
    /// 校验持有者类型
    /// </summary>
    /// <param name="ownerType">持有者类型</param>
    /// <returns>校验并 trim 后的类型</returns>
    private static string ValidateOwnerType(string ownerType)
    {
        string? trimmed = ownerType?.Trim();
        if (trimmed is null || trimmed.Length == 0 || !AttachmentOwnerTypeEnum.IsValid(trimmed))
        {
            throw new UserFriendlyException($"持有者类型{ownerType}有误!");
        }

        return trimmed;
    }
}
