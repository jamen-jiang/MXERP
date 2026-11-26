using Snow.Exceptions;

namespace MXERP.Domain.Common.Enums;

/// <summary>
/// 枚举相关扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 将字符串转换为枚举的名称(校验必须是枚举成员, 忽略大小写与首尾空白)
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="value">待转换的字符串</param>
    /// <param name="fieldName">字段名称(用于错误提示)</param>
    /// <returns>枚举成员的规范名称</returns>
    public static string ToEnumName<TEnum>(this string? value, string fieldName = "值") where TEnum : struct, Enum
    {
        string? name = Enum.GetNames<TEnum>().FirstOrDefault(n => string.Equals(n, value?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (name is null)
        {
            throw new UserFriendlyException(fieldName + "无效: " + value + "(可选值: " + string.Join("/", Enum.GetNames<TEnum>()) + ")");
        }

        return name;
    }
}
