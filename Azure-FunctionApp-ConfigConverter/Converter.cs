using System.Text.Json;

namespace Azure_FA_Configuration_Converter
{
    public interface IConverter
    {
        void LocalToPipeline();
        void PipelineToAzureFaConfig();
    }

    public class Converter : IConverter
    {
        private enum WriteMode
        {
            Overwrite = 1,
            CreateNew = 2
        }

        private readonly string folderName;
        private readonly string pipelineFolderName;
        private readonly string functionAppFolderName;
        private readonly string writeMode;
        private readonly bool isSorted;
        private readonly bool reversedSort;
        private readonly List<EnvironmentMapping> envMappings;
        private readonly Dictionary<string, Dictionary<string, string>> variables;
        private readonly List<string> ignoreVariables;

        public Converter()
        {
            var configData = DeserializeConfigData("converter.json");
            folderName = configData.FolderName;
            pipelineFolderName = configData.PipelineFolderName;
            functionAppFolderName = configData.FunctionAppFolderName;
            writeMode = configData.WriteMode;
            isSorted = configData.IsSorted;
            reversedSort = configData.ReversedSort;
            envMappings = configData.ConfigPath.Env;
            variables = configData.Variables;
            ignoreVariables = configData.IgnoreVariables;
        }

        public void LocalToPipeline()
        {
            ProcessFiles(GetOutputFolderPathLocalToPipeline, ".txt", ConvertLocalToPipeline);
        }

        public void PipelineToAzureFaConfig()
        {
            ProcessFiles(GetOutputFolderPathPipelineToAzureFa, ".json", ConvertPipelineToAzureFaConfig);
        }

        private string GetOutputFolderPathLocalToPipeline(string environment)
        {
            if (Enum.TryParse(writeMode, out WriteMode mode) && mode == WriteMode.CreateNew)
            {
                var timestamp = (int)System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return $"{folderName}_{timestamp}/{environment}_environment/{pipelineFolderName}";
            }
            else
            {
                return $"{folderName}/{environment}_environment/{pipelineFolderName}";
            }
        }

        private string GetOutputFolderPathPipelineToAzureFa(string environment)
        {
            if (Enum.TryParse(writeMode, out WriteMode mode) && mode == WriteMode.CreateNew)
            {
                var timestamp = (int)System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return $"{folderName}_{timestamp}/{environment}_environment/{functionAppFolderName}";
            }
            else
            {
                return $"{folderName}/{environment}_environment/{functionAppFolderName}";
            }
        }

        private void ProcessFiles(System.Func<string, string> getOutputFolderPathFunc, string fileExtension, System.Func<string, string, string> convertFunc)
        {
            foreach (var envMapping in envMappings)
            {
                var environmentNames = envMapping.Names;
                var pathMappings = envMapping.Path;

                foreach (var environment in environmentNames)
                {
                    var outputFolder = getOutputFolderPathFunc(environment);
                    Directory.CreateDirectory(outputFolder);

                    foreach (var pathFilename in pathMappings)
                    {
                        var path = pathFilename[0];
                        var filename = pathFilename[1];
                        var data = File.ReadAllText(path);
                        var convertedData = convertFunc(data, environment);

                        var outputFileName = Path.Combine(outputFolder, $"{environment}_{filename}{fileExtension}");
                        File.WriteAllText(outputFileName, convertedData);
                    }
                }
            }
        }

        private string ConvertLocalToPipeline(string data, string environment)
        {
            var sourceConfig = DeserializeSourceConfig(data);
            var parsedData = sourceConfig.Values.ToList();
            var formattedStrings = new List<string>();
            foreach (var item in SortedItems(parsedData))
            {
                var key = item.Key;
                var value = item.Value;
                if (!ignoreVariables.Contains(key))
                {
                    var formattedValue = ReplaceVariables(key, value, environment, variables);
                    formattedStrings.Add($"-{key} \"{formattedValue}\"");
                }
            }
            return string.Join(" ", formattedStrings);
        }

        private string ConvertPipelineToAzureFaConfig(string data, string environment)
        {
            var sourceConfig = DeserializeSourceConfig(data);
            var values = sourceConfig.Values.ToList();
            var result = new List<object>();
            foreach (var item in SortedItems(values))
            {
                var key = item.Key;
                var value = item.Value;
                if (!ignoreVariables.Contains(key))
                {
                    var formattedValue = ReplaceVariables(key, value, environment, variables);
                    result.Add(new { name = key, value = formattedValue, slotSetting = false });
                }
            }
            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }

        private string ReplaceVariables(string key, string value, string environment, Dictionary<string, Dictionary<string, string>> variables)
        {
            if (variables.ContainsKey(environment) && variables[environment].ContainsKey(key))
            {
                value = variables[environment][key];
                if (value.StartsWith("#$") && value.EndsWith("$#"))
                {
                    var extractedValue = value.Substring(2, value.Length - 4);
                    value = variables[extractedValue][key];
                }
                return value;
            }
            else
            {
                return value;
            }
        }

        private IEnumerable<KeyValuePair<string, string>> SortedItems(List<KeyValuePair<string, string>> list)
        {
            IEnumerable<KeyValuePair<string, string>> items = isSorted ? list.OrderBy(x => x.Key) : list;
            return reversedSort ? items.Reverse() : items;
        }

        private ConfigData DeserializeConfigData(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<ConfigData>(json);
        }

        private SourceConfig DeserializeSourceConfig(string json)
        {
            return JsonSerializer.Deserialize<SourceConfig>(json);
        }
    }
}
