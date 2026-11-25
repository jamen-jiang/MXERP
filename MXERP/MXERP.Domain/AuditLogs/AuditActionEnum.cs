using System.ComponentModel;

namespace MXERP.Domain.AuditLogs;

/// <summary>
/// 审核动作(对应 Java 的 AuditActionEnum, 以编码字符串存储)
/// </summary>
public enum AuditActionEnum
{
    /// <summary>
    /// 修改
    /// </summary>
    [Description("修改")]
    Modify,

    /// <summary>
    /// 打印
    /// </summary>
    [Description("打印")]
    Print,

    /// <summary>
    /// 提交
    /// </summary>
    [Description("提交")]
    Commit,

    /// <summary>
    /// 弃提
    /// </summary>
    [Description("弃提")]
    UnCommit,

    /// <summary>
    /// 拒审
    /// </summary>
    [Description("拒审")]
    RejectCheck,

    /// <summary>
    /// 审核
    /// </summary>
    [Description("审核")]
    Check,

    /// <summary>
    /// 弃审
    /// </summary>
    [Description("弃审")]
    UnCheck,

    /// <summary>
    /// 拒通
    /// </summary>
    [Description("拒通")]
    RejectPass,

    /// <summary>
    /// 通过
    /// </summary>
    [Description("通过")]
    Pass,

    /// <summary>
    /// 弃通
    /// </summary>
    [Description("弃通")]
    UnPass,

    /// <summary>
    /// 拒完
    /// </summary>
    [Description("拒完")]
    RejectFinish,

    /// <summary>
    /// 完成
    /// </summary>
    [Description("完成")]
    Finish,

    /// <summary>
    /// 弃完
    /// </summary>
    [Description("弃完")]
    UnFinish,

    /// <summary>
    /// 取消
    /// </summary>
    [Description("取消")]
    Cancel,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete
}
