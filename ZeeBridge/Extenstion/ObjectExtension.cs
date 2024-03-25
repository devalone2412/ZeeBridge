using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZeeBridge.Extenstion;

/// <summary>
///     This static class contains extension methods for the object type.
/// </summary>
internal static class ObjectExtension
{
    /// <summary>
    ///     Serializes the given object into a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    internal static string ToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }
    
    /// <summary>
    ///     Serializes the given object into a JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="jsonSetting">Provides options to be used with <see cref="JsonSerializer"/></param>
    /// <returns>A JSON string representation of the object.</returns>
    internal static string ToJson(this object obj, JsonSerializerOptions jsonSetting)
    {
        return JsonSerializer.Serialize(
            obj,
            jsonSetting
        );
    }

    /// <summary>
    ///     Deserialize the given json text to Key-Value pair.
    /// </summary>
    /// <param name="json">The input which need to Deserialize.</param>
    /// <returns>Return type of T</returns>
    internal static T ParseObject<T>(this string json)
    {
        return JsonSerializer.Deserialize<T?>(json)!;
    }

    /// <summary>
    ///     Deserialize the given json text to Key-Value pair.
    /// </summary>
    /// <param name="json">The input which need to Deserialize.</param>
    /// <param name="jsonSetting">Provides options to be used with <see cref="JsonSerializer"/></param>
    /// <returns>Return type of T</returns>
    internal static T ParseObject<T>(this string json, JsonSerializerOptions jsonSetting)
    {
        return JsonSerializer.Deserialize<T?>(
            json,
            jsonSetting
        )!;
    }
}