using System.ComponentModel;

namespace MXERP.Domain.Attachments;

/// <summary>
/// 附件类型(对应 Java 的 AttachmentOwnerTypeEnum, 以编码字符串存储)
/// </summary>
public enum AttachmentOwnerTypeEnum
{
    /// <summary>
    /// 批量订单项
    /// </summary>
    [Description("批量订单项")]
    BulkOrderItem,

    /// <summary>
    /// 销售镜种
    /// </summary>
    [Description("销售镜种")]
    SalesLens,

    /// <summary>
    /// 客人镜种
    /// </summary>
    [Description("客人镜种")]
    CustomerLens,

    /// <summary>
    /// 邮件
    /// </summary>
    [Description("邮件")]
    Email,

    /// <summary>
    /// 销售镜种打唛图片
    /// </summary>
    [Description("销售镜种打唛图片")]
    SalesLensMarking,

    /// <summary>
    /// 产品
    /// </summary>
    [Description("产品")]
    Spu
}
