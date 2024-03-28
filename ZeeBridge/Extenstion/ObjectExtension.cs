using Newtonsoft.Json;

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
        return JsonConvert.SerializeObject(obj);
    }

    internal static T FromJson<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}