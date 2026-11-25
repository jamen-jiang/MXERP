using MXERP.Domain.Common.Enums;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性工厂接口
/// </summary>
public interface IAttributeFactory
{
    /// <summary>
    /// 创建属性
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
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>属性聚合</returns>
    Task<Attribute> CreateAsync(ProductTypeEnum productType, AttributeScopeEnum scope, string code, string name, bool isKey, InputTypeEnum inputType, DataTypeEnum dataType, bool isRequired, bool isFilter, int? sort, string? remark, CancellationToken cancellationToken = default);
}
