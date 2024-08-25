using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleReceiptProcessor.Controllers.Converters;

public class CustomTimeSpanConverter : JsonConverter<TimeSpan>
{
    private const string Format = @"hh\:mm";

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (TimeSpan.TryParseExact(reader.GetString(), Format, null, out var timeSpan))
            {
                return timeSpan;
            }
        }

        throw new JsonException("Invalid time format, expected HH:mm.");
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}