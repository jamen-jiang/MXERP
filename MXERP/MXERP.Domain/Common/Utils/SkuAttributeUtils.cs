using System.Text;

namespace MXERP.Domain.Common.Utils;

/// <summary>
/// 关键属性工具类
/// </summary>
public static class SkuAttributeUtils
{
    /// <summary>
    /// 关键属性拼成文本（格式：key1:value1|key2:value2）
    /// </summary>
    /// <param name="keyAttributes">关键属性值（可空）</param>
    /// <returns>拼接后的文本, 无属性时返回空字符串</returns>
    public static string ToText(SortedDictionary<string, object?>? keyAttributes)
    {
        if (keyAttributes is null || keyAttributes.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder builder = new StringBuilder();
        bool isFirst = true;
        foreach (KeyValuePair<string, object?> pair in keyAttributes)
        {
            if (!isFirst)
            {
                builder.Append('|');
            }

            builder.Append(pair.Key).Append(':').Append(pair.Value?.ToString() ?? "null");
            isFirst = false;
        }

        return builder.ToString();
    }
}
