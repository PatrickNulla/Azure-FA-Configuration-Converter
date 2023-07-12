using System.Text.Json.Serialization;

namespace Azure_FA_Configuration_Converter
{
    public class ConfigData
    {
        [JsonPropertyName("folderName")]
        public string FolderName { get; set; }

        [JsonPropertyName("pipelineFolderName")]
        public string PipelineFolderName { get; set; }

        [JsonPropertyName("functionAppFolderName")]
        public string FunctionAppFolderName { get; set; }

        [JsonPropertyName("writeMode")]
        public string WriteMode { get; set; }

        [JsonPropertyName("isSorted")]
        public bool IsSorted { get; set; }

        [JsonPropertyName("reversedSort")]
        public bool ReversedSort { get; set; }

        [JsonPropertyName("configPath")]
        public ConfigPath ConfigPath { get; set; }

        [JsonPropertyName("variables")]
        public Dictionary<string, Dictionary<string, string>> Variables { get; set; }

        [JsonPropertyName("ignoreVariables")]
        public List<string> IgnoreVariables { get; set; }
    }
}
