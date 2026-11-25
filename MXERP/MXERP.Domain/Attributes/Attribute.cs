using Snow.Domain.Entities.Auditing;
using Snow.Exceptions;
using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性
/// </summary>
public class Attribute : AuditedAggregateRoot
{
    /// <summary>
    /// 产品类型
    /// </summary>
    public string ProductType { get; private set; } = string.Empty;

    /// <summary>
    /// 范围
    /// </summary>
    public string Scope { get; private set; } = string.Empty;

    /// <summary>
    /// 属性代码
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// 属性名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 是否关键属性
    /// </summary>
    public bool IsKey { get; private set; }

    /// <summary>
    /// 输入类型
    /// </summary>
    public string InputType { get; private set; } = string.Empty;

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; private set; } = string.Empty;

    /// <summary>
    /// 是否必填
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// 是否用于筛选
    /// </summary>
    public bool IsFilter { get; private set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 属性项列表
    /// </summary>
    public virtual ICollection<AttributeItem> AttributeItems { get; private set; } = new List<AttributeItem>();

    /// <summary>
    /// 校验规则列表
    /// </summary>
    public virtual ICollection<AttributeValidateRule> AttributeValidateRules { get; private set; } = new List<AttributeValidateRule>();

    /// <summary>
    /// 供EF使用的无参构造器
    /// </summary>
    protected Attribute()
    {
    }

    /// <summary>
    /// 构造器
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="scope">范围</param>
    /// <param name="code">属性代码</param>
    /// <param name="name">属性名称</param>
    /// <param name="isKey">是否关键属性</param>
    /// <param name="inputType">输入类型</param>
    /// <param name="dataType">数据类型</param>
    /// <param name="isRequired">是否必填</param>
    /// <param name="isFilter">是否用于筛选</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    internal Attribute(
        string productType,
        string scope,
        string code,
        string name,
        bool isKey,
        string inputType,
        string dataType,
        bool isRequired,
        bool isFilter,
        int? sort,
        string? remark)
    {
        string? trimmedProductType = productType?.Trim();
        string? trimmedScope = scope?.Trim();
        string? trimmedCode = code?.Trim();
        string? trimmedName = name?.Trim();
        if (string.IsNullOrEmpty(trimmedProductType))
        {
            throw new UserFriendlyException("产品类型不能为空");
        }

        if (string.IsNullOrEmpty(trimmedScope))
        {
            throw new UserFriendlyException("范围不能为空");
        }

        if (string.IsNullOrEmpty(trimmedCode))
        {
            throw new UserFriendlyException("属性代码不能为空");
        }

        if (string.IsNullOrEmpty(trimmedName))
        {
            throw new UserFriendlyException("属性名称不能为空");
        }

        ValidateProductType(trimmedProductType);
        ValidateScope(trimmedScope);
        ValidateInputType(inputType);
        ValidateDataType(dataType);

        ProductType = trimmedProductType;
        Scope = trimmedScope;
        Code = trimmedCode;
        Name = trimmedName;
        IsKey = isKey;
        InputType = inputType;
        DataType = dataType;
        IsRequired = isRequired;
        IsFilter = isFilter;
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 获取属性项列表(只读)
    /// </summary>
    /// <returns>不可变列表</returns>
    public IReadOnlyList<AttributeItem> GetAttributeItems()
    {
        return AttributeItems.ToList().AsReadOnly();
    }

    /// <summary>
    /// 获取校验规则列表(只读)
    /// </summary>
    /// <returns>不可变列表</returns>
    public IReadOnlyList<AttributeValidateRule> GetAttributeValidateRules()
    {
        return AttributeValidateRules.ToList().AsReadOnly();
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="name">属性名称</param>
    /// <param name="inputType">输入类型</param>
    /// <param name="isRequired">是否必填</param>
    /// <param name="isFilter">是否用于筛选</param>
    /// <param name="sort">排序</param>
    /// <param name="remark">备注</param>
    public void Modify(string name, string inputType, bool isRequired, bool isFilter, int? sort, string? remark)
    {
        string? trimmedName = name?.Trim();
        if (string.IsNullOrEmpty(trimmedName))
        {
            throw new UserFriendlyException("属性名称不能为空");
        }

        ValidateInputType(inputType);
        ValidateDataType(DataType);

        Name = trimmedName;
        InputType = inputType;
        IsRequired = isRequired;
        IsFilter = isFilter;
        Sort = sort;
        Remark = remark;
    }

    /// <summary>
    /// 添加属性项
    /// </summary>
    /// <param name="value">属性值</param>
    /// <param name="name">属性值名称</param>
    /// <param name="isDefault">是否默认</param>
    /// <param name="sort">排序</param>
    public void AddItem(string value, string name, bool isDefault, int? sort)
    {
        if (IsExistItem(value))
        {
            throw new UserFriendlyException("属性" + Name + ":属性项" + value + " 已存在!");
        }

        AttributeItems.Add(new AttributeItem(value, name, isDefault, sort));
    }

    /// <summary>
    /// 修改属性项
    /// </summary>
    /// <param name="itemId">属性项Id</param>
    /// <param name="name">属性值名称</param>
    /// <param name="isDefault">是否默认</param>
    /// <param name="sort">排序</param>
    public void ModifyItem(long itemId, string name, bool isDefault, int? sort)
    {
        AttributeItem? item = AttributeItems.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            throw new UserFriendlyException("属性" + Name + ":属性项不存在!");
        }

        item.Modify(name, isDefault, sort);
    }

    /// <summary>
    /// 删除属性项
    /// </summary>
    /// <param name="itemId">属性项Id</param>
    public void DeleteItem(long itemId)
    {
        AttributeItem? item = AttributeItems.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return;
        }

        item.Delete();
    }

    /// <summary>
    /// 添加校验规则
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <param name="errorMessage">错误提示</param>
    /// <param name="sort">排序</param>
    public void AddValidateRule(string expression, string errorMessage, int? sort)
    {
        if (IsExistValidateRule(expression))
        {
            throw new UserFriendlyException("属性" + Name + ":校验规则已存在!");
        }

        AttributeValidateRules.Add(new AttributeValidateRule(expression, errorMessage, sort));
    }

    /// <summary>
    /// 修改校验规则
    /// </summary>
    /// <param name="validateRuleId">校验规则Id</param>
    /// <param name="expression">表达式</param>
    /// <param name="errorMessage">错误提示</param>
    /// <param name="sort">排序</param>
    public void ModifyValidateRule(long validateRuleId, string expression, string errorMessage, int? sort)
    {
        AttributeValidateRule? validateRule = AttributeValidateRules.FirstOrDefault(r => r.Id == validateRuleId);
        if (validateRule == null)
        {
            throw new UserFriendlyException("属性" + Name + ":校验规则不存在!");
        }

        validateRule.Modify(expression, errorMessage, sort);
    }

    /// <summary>
    /// 删除校验规则
    /// </summary>
    /// <param name="validateRuleId">校验规则Id</param>
    public void DeleteValidateRule(long validateRuleId)
    {
        AttributeValidateRule? validateRule = AttributeValidateRules.FirstOrDefault(r => r.Id == validateRuleId);
        if (validateRule == null)
        {
            return;
        }

        validateRule.Delete();
    }

    /// <summary>
    /// 校验产品类型
    /// </summary>
    /// <param name="productType">产品类型</param>
    private static void ValidateProductType(string productType)
    {
        if (!ProductTypeEnum.IsValid(productType))
        {
            throw new UserFriendlyException("产品类型" + productType + "有误!");
        }
    }

    /// <summary>
    /// 校验范围
    /// </summary>
    /// <param name="scope">范围</param>
    private static void ValidateScope(string scope)
    {
        if (!AttributeScopeEnum.IsValid(scope))
        {
            throw new UserFriendlyException("范围" + scope + "有误!");
        }
    }

    /// <summary>
    /// 校验输入类型
    /// </summary>
    /// <param name="inputType">输入类型</param>
    private static void ValidateInputType(string inputType)
    {
        if (!InputTypeEnum.IsValid(inputType))
        {
            throw new UserFriendlyException("输入类型" + inputType + "有误!");
        }
    }

    /// <summary>
    /// 校验数据类型
    /// </summary>
    /// <param name="dataType">数据类型</param>
    private static void ValidateDataType(string dataType)
    {
        if (!DataTypeEnum.IsValid(dataType))
        {
            throw new UserFriendlyException("数据类型" + dataType + "有误!");
        }
    }

    /// <summary>
    /// 判断属性项是否存在
    /// </summary>
    /// <param name="value">属性值</param>
    /// <returns>是否存在</returns>
    private bool IsExistItem(string value)
    {
        return AttributeItems.Any(i => i.Value == value && !i.IsDeleted);
    }

    /// <summary>
    /// 判断校验规则是否存在
    /// </summary>
    /// <param name="expression">表达式</param>
    /// <returns>是否存在</returns>
    private bool IsExistValidateRule(string expression)
    {
        return AttributeValidateRules.Any(r => r.Expression == expression && !r.IsDeleted);
    }
}
