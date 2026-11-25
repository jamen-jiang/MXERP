using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MXERP.Infrastructure.EFCore.Configurations;

/// <summary>
/// SortedDictionary&lt;string, object?&gt; 与 JSON 字符串互转的 EF 映射扩展
/// </summary>
public static class SortedDictionaryJsonExtensions
{
    private static readonly ValueConverter<SortedDictionary<string, object?>?, string?> Converter = new(
        v => v == null ? null : JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
        v => v == null ? null : JsonSerializer.Deserialize<SortedDictionary<string, object?>>(v, JsonSerializerOptions.Default));

    private static readonly ValueComparer<SortedDictionary<string, object?>?> Comparer = new(
        (a, b) => JsonSerializer.Serialize(a, JsonSerializerOptions.Default) == JsonSerializer.Serialize(b, JsonSerializerOptions.Default),
        v => v == null ? 0 : JsonSerializer.Serialize(v, JsonSerializerOptions.Default).GetHashCode(),
        v => v == null ? null : JsonSerializer.Deserialize<SortedDictionary<string, object?>>(JsonSerializer.Serialize(v, JsonSerializerOptions.Default), JsonSerializerOptions.Default));

    /// <summary>
    /// 配置属性以 JSON 字符串存储, 并设置值比较器
    /// </summary>
    /// <param name="propertyBuilder">属性构建器</param>
    /// <returns>属性构建器</returns>
    public static PropertyBuilder<SortedDictionary<string, object?>?> HasSortedDictionaryJsonConversion(this PropertyBuilder<SortedDictionary<string, object?>?> propertyBuilder)
    {
        propertyBuilder.HasConversion(Converter);
        propertyBuilder.Metadata.SetValueComparer(Comparer);
        return propertyBuilder;
    }
}
