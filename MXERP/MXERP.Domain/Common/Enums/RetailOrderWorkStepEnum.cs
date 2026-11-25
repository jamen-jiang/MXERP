using System.ComponentModel;

namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 散镜订单工序
/// </summary>
public enum RetailOrderWorkStepEnum
{
    /// <summary>
    /// 接受订单
    /// </summary>
    [Description("接受订单")]
    JieShouDingDan,

    /// <summary>
    /// 送仓库
    /// </summary>
    [Description("送仓库")]
    SongCangKu,

    /// <summary>
    /// 送包装
    /// </summary>
    [Description("送包装")]
    SongBaoZhuang,

    /// <summary>
    /// 出货
    /// </summary>
    [Description("出货")]
    ChuHuo,

    /// <summary>
    /// 取消
    /// </summary>
    [Description("取消")]
    QuXiao
}
