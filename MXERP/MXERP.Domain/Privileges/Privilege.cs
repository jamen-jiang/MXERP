using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Privileges;

/// <summary>
/// 权限(对应 Java Privilege)
/// </summary>
public class Privilege : DeletionAuditedAggregateRoot
{
    /// <summary>
    /// 权限对象
    /// </summary>
    public string Master { get; private set; } = string.Empty;
    /// <summary>
    /// 权限对象值
    /// </summary>
    public long MasterValue { get; private set; }
    /// <summary>
    /// 权限通道
    /// </summary>
    public string Access { get; private set; } = string.Empty;
    /// <summary>
    /// 权限通道值
    /// </summary>
    public long AccessValue { get; private set; }
    /// <summary>
    /// 操作
    /// </summary>
    public int Operation { get; private set; }

    /// <summary>
    /// 构造器(供 EF 使用)
    /// </summary>
    protected Privilege()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="master">权限对象</param>
    /// <param name="masterValue">权限对象值</param>
    /// <param name="access">权限通道</param>
    /// <param name="accessValue">权限通道值</param>
    public Privilege(string master, long masterValue, string access, long accessValue)
    {
        Master = master;
        MasterValue = masterValue;
        Access = access;
        AccessValue = accessValue;
        Operation = 0;
    }




}
