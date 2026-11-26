using MXERP.Domain.Common.Enums;
using Snow.Domain.Repositories;

namespace MXERP.Domain.Attributes;

/// <summary>
/// 属性仓储接口
/// </summary>
public interface IAttributeRepository : IRepository<Attribute>
{
    /// <summary>
    /// 按产品类型 + 范围 + 代码查询
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="scope">范围</param>
    /// <param name="code">属性代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>属性, 不存在返回 null</returns>
    Task<Attribute?> GetByProductTypeAndScopeAndCodeAsync(string productType, string scope, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按产品类型 + 范围查询列表
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="scope">范围</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>属性列表</returns>
    Task<List<Attribute>> GetListByProductTypeAndScopeAsync(string productType, string scope, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断属性代码是否存在(排除指定主键)
    /// </summary>
    /// <param name="productType">产品类型</param>
    /// <param name="scope">范围</param>
    /// <param name="code">属性代码</param>
    /// <param name="id">排除的主键Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>true 表示已存在</returns>
    Task<bool> IsExistByProductTypeAndScopeAndCodeExcludeIdAsync(string productType, string scope, string code, long? id = null, CancellationToken cancellationToken = default);
}
