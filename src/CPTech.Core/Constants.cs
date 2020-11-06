using System.Text.Encodings.Web;
using System.Text.Json;

namespace CPTech.Core
{
    public class Constants
    {
        public static JsonSerializerOptions JsonSerializerOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
