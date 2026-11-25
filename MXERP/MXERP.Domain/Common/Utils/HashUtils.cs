using System.Security.Cryptography;
using System.Text;

namespace MXERP.Domain.Common.Utils;

/// <summary>
/// 哈希工具
/// </summary>
public static class HashUtils
{
    /// <summary>
    /// 计算字符串的MD5哈希值
    /// </summary>
    /// <param name="input">输入字符串</param>
    /// <returns>小写十六进制MD5, 输入为空返回空字符串</returns>
    public static string ComputeMd5(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        StringBuilder builder = new StringBuilder(hashBytes.Length * 2);
        foreach (byte b in hashBytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}
