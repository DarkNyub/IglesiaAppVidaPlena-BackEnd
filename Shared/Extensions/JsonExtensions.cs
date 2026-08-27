using System.Text.Json;

namespace IglesiaBackend.Shared.Extensions;

public static class JsonExtensions
{
    public static string? JsonField(this string json, string field)
    {
        if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(field))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(field, out var value))
            {
                return value.ToString();
            }
        }
        catch (JsonException)
        {
            // Invalid JSON, return null
        }

        return null;
    }
}