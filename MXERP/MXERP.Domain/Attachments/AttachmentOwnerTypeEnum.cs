namespace MXERP.Domain.Attachments;

/// <summary>
/// 附件类型(对应 Java 的 AttachmentOwnerTypeEnum, 以编码字符串存储)
/// </summary>
public static class AttachmentOwnerTypeEnum
{
    /// <summary>
    /// 批量订单项
    /// </summary>
    public const string BulkOrderItem = "BulkOrderItem";
    /// <summary>
    /// 销售镜种
    /// </summary>
    public const string SalesLens = "SalesLens";
    /// <summary>
    /// 客人镜种
    /// </summary>
    public const string CustomerLens = "CustomerLens";
    /// <summary>
    /// 邮件
    /// </summary>
    public const string Email = "Email";
    /// <summary>
    /// 销售镜种打唛图片
    /// </summary>
    public const string SalesLensMarking = "SalesLensMarking";
    /// <summary>
    /// 产品
    /// </summary>
    public const string Spu = "Spu";

    /// <summary>
    /// 全部编码
    /// </summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        BulkOrderItem, SalesLens, CustomerLens, Email, SalesLensMarking, Spu
    };

    /// <summary>
    /// 编码是否有效
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns>有效返回 true</returns>
    public static bool IsValid(string? code)
    {
        return code is not null && All.Contains(code);
    }
}
