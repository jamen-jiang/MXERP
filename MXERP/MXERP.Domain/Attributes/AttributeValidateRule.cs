using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性校验规则
/// </summary>
public class AttributeValidateRule : DeletionAuditedEntity
{
    /// <summary>
    /// 属性Id
    /// </summary>
    public long? AttributeId { get; private set; }

    /// <summary>
    /// 表达式
    /// </summary>
    public string Expression { get; private set; } = string.Empty;

    /// <summary>
    /// 错误提示
    /// </summary>
    public string ErrorMessage { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; private set; }

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected AttributeValidateRule()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <param name="errorMessage">错误提示</param>
    /// <param name="sort">排序</param>
    internal AttributeValidateRule(string expression, string errorMessage, int? sort)
    {
        string? trimmedExpression = expression?.Trim();
        string? trimmedErrorMessage = errorMessage?.Trim();
        if (string.IsNullOrEmpty(trimmedExpression))
        {
            throw new UserFriendlyException("表达式不能为空");
        }

        if (string.IsNullOrEmpty(trimmedErrorMessage))
        {
            throw new UserFriendlyException("错误提示不能为空");
        }

        Expression = trimmedExpression;
        ErrorMessage = trimmedErrorMessage;
        Sort = sort;
    }

    /// <summary>
    /// 修改校验规则
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <param name="errorMessage">错误提示</param>
    /// <param name="sort">排序</param>
    public void Modify(string expression, string errorMessage, int? sort)
    {
        string? trimmedExpression = expression?.Trim();
        string? trimmedErrorMessage = errorMessage?.Trim();
        if (string.IsNullOrEmpty(trimmedExpression))
        {
            throw new UserFriendlyException("表达式不能为空");
        }

        if (string.IsNullOrEmpty(trimmedErrorMessage))
        {
            throw new UserFriendlyException("错误提示不能为空");
        }

        Expression = trimmedExpression;
        ErrorMessage = trimmedErrorMessage;
        Sort = sort;
    }
}
