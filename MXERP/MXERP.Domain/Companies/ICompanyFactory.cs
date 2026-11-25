namespace MXERP.Domain.Companies;

/// <summary>
/// 公司工厂接口
/// </summary>
public interface ICompanyFactory
{
    /// <summary>
    /// 创建公司
    /// </summary>
    /// <param name="region">区域</param>
    /// <param name="group">组别(可空)</param>
    /// <param name="code">代码</param>
    /// <param name="name">名称</param>
    /// <param name="shortName">简称(可空)</param>
    /// <param name="numberCode">数字代码(可空)</param>
    /// <param name="telephone">电话号码(可空)</param>
    /// <param name="fax">传真(可空)</param>
    /// <param name="email">邮箱(可空)</param>
    /// <param name="areaCode">地区代码(可空)</param>
    /// <param name="postCode">邮政编码(可空)</param>
    /// <param name="currencyId">币种Id</param>
    /// <param name="address">地址(可空)</param>
    /// <param name="billingAddress">账户地址(可空)</param>
    /// <param name="sort">排序(可空)</param>
    /// <param name="remark">备注(可空)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回公司</returns>
    Task<Company> CreateAsync(string region, string? group, string code, string name, string? shortName, string? numberCode, string? telephone, string? fax, string? email, string? areaCode, string? postCode, long currencyId, string? address, string? billingAddress, int? sort, string? remark, CancellationToken cancellationToken = default);
}
