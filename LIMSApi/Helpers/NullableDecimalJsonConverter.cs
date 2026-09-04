using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LIMSApi.Helpers
{
    /// <summary>
    /// Custom System.Text.Json converter for Nullable<decimal>.
    /// Gracefully treats empty/whitespace strings as null instead of throwing JsonException.
    /// </summary>
    public class NullableDecimalJsonConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDecimal();

            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                    return null;

                if (decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var invResult))
                    return invResult;

                if (decimal.TryParse(str, out var localResult))
                    return localResult;
            }

            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to decimal?.");
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
