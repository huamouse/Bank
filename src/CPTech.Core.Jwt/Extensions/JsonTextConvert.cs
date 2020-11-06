using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace CPTech.Extensions
{
    public class JsonTextConvert
    {
        public class DateTimeConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => DateTime.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public class DateTimeNullableConverter : JsonConverter<DateTime?>
        {
            public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => string.IsNullOrEmpty(reader.GetString()) ? default(DateTime?) : DateTime.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
                => writer.WriteStringValue(value?.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public class UpperCaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => name.ToUpper();
        }

        public class UnderLineNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) =>
                string.Join("", name.ToArray().Select((e, i) => 
                    i != 0 && Regex.IsMatch(e.ToString(), "[A-Z]") ? "_" + e.ToString().ToLower() : e.ToString()).ToArray());
        }
    }
}
