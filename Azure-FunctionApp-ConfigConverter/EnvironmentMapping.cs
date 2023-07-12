using System.Text.Json.Serialization;

namespace Azure_FA_Configuration_Converter
{
    public class EnvironmentMapping
    {
        [JsonPropertyName("names")]
        public List<string> Names { get; set; }

        [JsonPropertyName("path")]
        public List<string[]> Path { get; set; }
    }
}
