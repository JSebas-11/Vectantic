using System.Text.Json;

namespace Vectantic.Semantic.Internal.Extensions;

internal static class JsonExtensions {
    internal static string? GetStringValue(this JsonElement element, string prop) {
        if (element.TryGetProperty(prop, out var value) && 
            value.ValueKind == JsonValueKind.String)
            return value.GetString();

        return null;
    }

    internal static JsonElement? GetObject(this JsonElement element, string prop) {
        if (element.TryGetProperty(prop, out var value) && value.ValueKind == JsonValueKind.Object)
            return value;

        return null;
    }
}