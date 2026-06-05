using System.Text.Json;

namespace Vectantic.Semantic.Internal.Extensions;

internal static class JsonExtensions {
    internal static string? GetStringValue(this JsonElement element, string? prop) {
        if (string.IsNullOrWhiteSpace(prop)) return null;

        if (element.TryGetProperty(prop, out var value) && 
            value.ValueKind == JsonValueKind.String)
            return value.GetString();

        return null;
    }

     public static int? GetInt(this JsonElement element, string? prop) {
        if (string.IsNullOrWhiteSpace(prop)) return null;

        if (element.TryGetProperty(prop, out var value) && value.ValueKind == JsonValueKind.Number)
            return value.GetInt32();

        return null;
    }

    internal static JsonElement? GetObject(this JsonElement element, string? prop) {
        if (string.IsNullOrWhiteSpace(prop)) return null;

        if (element.TryGetProperty(prop, out var value) && value.ValueKind == JsonValueKind.Object)
            return value;

        return null;
    }
}