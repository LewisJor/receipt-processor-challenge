using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleReceiptProcessor.Controllers.Converters;

public class TimeSpanConverter : JsonConverter<TimeSpan>
{
    private const string TimeFormat = @"hh\:mm";

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? timeString = reader.GetString();
        if (TimeSpan.TryParseExact(timeString, TimeFormat, null, out TimeSpan timeSpan))
        {
            return timeSpan;
        }
        throw new JsonException($"Unable to convert \"{timeString}\" to {nameof(TimeSpan)}");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(TimeFormat));
    }
}