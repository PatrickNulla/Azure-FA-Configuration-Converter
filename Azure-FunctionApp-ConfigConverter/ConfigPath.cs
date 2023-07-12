using System.Text.Json.Serialization;

namespace Azure_FA_Configuration_Converter
{
    public class ConfigPath
    {
        [JsonPropertyName("env")]
        public List<EnvironmentMapping> Env { get; set; }
    }
}
