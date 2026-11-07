using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniCrm.Api.Helper;
public sealed class DayMonthYearDateOnlyConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "dd/MM/yyyy";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Ngày phải là chuỗi dạng {DateFormat}.");
        }

        var dateString = reader.GetString();
        if (DateOnly.TryParseExact(dateString, DateFormat, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }

        throw new JsonException($"Invalid date format. Expected format: {DateFormat}.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}