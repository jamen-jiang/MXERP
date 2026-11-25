namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性校验规则数据
/// </summary>
/// <param name="Expression">表达式</param>
/// <param name="ErrorMessage">错误提示</param>
/// <param name="Sort">排序</param>
public sealed record AttributeValidateRuleData(string Expression, string ErrorMessage, int? Sort);
