namespace MXERP.Domain.Stores;

/// <summary>
/// 门店联系人数据
/// </summary>
/// <param name="Name">姓名</param>
/// <param name="CompanyId">公司Id</param>
/// <param name="Telephone">电话(可空)</param>
/// <param name="Area">地区(可空)</param>
/// <param name="Address">地址(可空)</param>
public sealed record StoreContactData(string Name, long CompanyId, string? Telephone, string? Area, string? Address);
