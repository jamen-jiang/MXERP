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
        Master = ValidateMaster(master);
        MasterValue = masterValue;
        Access = ValidateAccess(access);
        AccessValue = accessValue;
        Operation = 0;
    }

    /// <summary>
    /// 校验权限对象
    /// </summary>
    /// <param name="master">权限对象</param>
    /// <returns>校验并 trim 后的权限对象</returns>
    private static string ValidateMaster(string master)
    {
        string? trimmed = master?.Trim();
        if (trimmed is null || trimmed.Length == 0 || !MasterEnum.IsValid(trimmed))
        {
            throw new UserFriendlyException($"权限对象{master}有误!");
        }

        return trimmed;
    }

    /// <summary>
    /// 校验权限通道
    /// </summary>
    /// <param name="access">权限通道</param>
    /// <returns>校验并 trim 后的权限通道</returns>
    private static string ValidateAccess(string access)
    {
        string? trimmed = access?.Trim();
        if (trimmed is null || trimmed.Length == 0 || !AccessEnum.IsValid(trimmed))
        {
            throw new UserFriendlyException($"权限通道{access}有误!");
        }

        return trimmed;
    }
}
