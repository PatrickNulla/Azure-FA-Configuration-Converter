# Local Function App Settings Converter [.NET]

A .NET console application used to convert Local Azure Function App Configuration to Azure DevOps Release Pipeline Configuration and Azure Function App Configuration.

[For the Python version, click here.](https://github.com/PatrickNulla/Azure-FunctionApp-Configuration-Converter)

## Prerequisites
- .NET 6.0 Runtime

## Usage
1. Place the `Azure-FunctionApp-ConfigConverter.exe` file and the configuration file (`converter.json`) in the same directory.

2. Open the `converter.json` file and update the configuration to match your specific requirements. The configuration consists of the following properties:
   - `folderName`: The base folder name.
   - `pipelineFolderName`: Folder name for the release pipeline generated config.
   - `functionAppFolderName`: Folder name for the Azure Function App generated config.
   - `writeMode`: Generation mode for the config, either Overwrite or CreateNew. Overwrite mode replaces the whole base folder. CreateNew mode generates a new folder with the Unix time appended to prevent replacing the previously generated configurations. (`Overwrite` | `CreateNew`)
   - `isSorted`: Sorts the generated configurations alphabetically. (`true` | `false`)
   - `reversedSort`: Reverses the order of the keys in the generated configurations. (`true` | `false`)
   - `configPath`: Contains environment mappings and file paths.
   - `variables`: Defines the value per configuration keys for specific environments (You could use the environment name to reuse the previously set value for that variable e.g. `#$Environment_Name$#`).
   - `ignoreVariables`: List of variables to be ignored in the generated configurations (Instead of removing the variable in the `variables` key, list out the unnecessary variables for documentation).

   Replace the placeholder values with your actual configuration paths, environments, and configuration keys.

   Example `converter.json` file:
   ```json
    {
        "folderName": "BaseFolderName",
        "pipelineFolderName": "ReleasePipelineFolderName",
        "functionAppFolderName": "FunctionAppFolderName",
        "writeMode": "Overwrite",
        "isSorted": true,
        "reversedSort": false, 
        "configPath": {
            "env": [
                {
                    "names": [
                        "list of your",
                        "environments"
                    ],
                    "path": [
                        ["Path to your local config (e.g. local.settings.json)", "File name to use upon generation"],
                        ["Path to your local config (e.g. local.settings.json)", "e.g. FunctionApp1"],
                        ["E:/Sample/Path/To/Your/local.settings.json", "FileNameItWillUse"]
                    ]
                }
            ]
        },
        "variables": {
            "one-of-your": {
                "VariableInYour": "sample-value-1"
            },
            "environments": {
                "Configuration": "sample-value-2"
            },
            "in-the-names-list": {
                "ThatHasAUniqueValuePerEnvironment": "#$one_of_your$#"
            }
        },
        "ignoreVariables": [
            "Configuration",
            "ThatHasAUniqueValuePerEnvironment"
        ]
    }
   ```

3. Open a terminal or command prompt and navigate to the directory containing the `Azure-FunctionApp-ConfigConverter.exe` and `converter.json` files.

4. Run the program by executing the following command:
   ```
   Azure-FunctionApp-ConfigConverter.exe (optional args: --pauseOnFinish)
   ```

5. The program will process the configurations and generate the formatted code and converted JSON files based on the specified paths and environment mappings.

6. The generated output will be placed in separate folders for each environment, as specified in the configuration.

## Customization
You can customize the behavior of the Converter program by modifying the configuration in the `converter.json` file according to your specific needs.

### Sample
For this example, let's base the configuration on the following scenario:
- You have 3 environments: `dev`, `staging`, and `production`.
- In production, you have 2 different payment methods: `Paypal` and `Stripe`.
- You have 2 Azure Function Apps: `Account` and `Payment` Function Apps.
- You have 3 different connection strings for each environment: `dev-connectionstring`, `staging-connectionstring`, and `production-connectionstring`.
- You have 2 different API keys for each payment method: `PaypalAPIKey` and `StripeAPIKey`.

For our scenario, the local.settings.json for the Payment Function App looks like this:
```json
{
  "IsEncrypted": false,
  "Values": {
    "DBConnectionString": "someValue",
    "Version": "someValue",
    "PaymentAPIKey": "someValue"
  }
}
```

For example, you have this converter configuration:
```json
    {
        "folderName": "Sample/Generated Configs",
        "pipelineFolderName": "Pipeline Configs",
        "functionAppFolderName": "Azure Function App Configs",
        "writeMode": "Overwrite",
        "isSorted": true,
        "reversedSort": false,   
        "configPath": {
            "env": [
                {
                    "names": [
                        "dev",
                        "staging"
                    ],
                    "path": [
                        ["D:/Sample/Local Settings/AccountFA/local.settings.json", "Account"],
                        ["D:/Sample/Local Settings/PaymentFA/local.settings.json", "Payment"]
                    ]
                },
                {
                    "names": [
                        "production-account"
                    ],
                    "path": [
                        ["D:/Sample/Local Settings/AccountFA/local.settings.json", "Account"]
                    ]
                },
                {
                    "names": [
                        "production-paypal"
                    ],
                    "path": [
                        ["D:/Sample/Local Settings/PaymentFA/local.settings.json", "Payment-Paypal"]
                    ]
                },
                {
                    "names": [
                        "production-stripe"
                    ],
                    "path": [
                        ["D:/Sample/Local Settings/PaymentFA/local.settings.json", "Payment-Stripe"]
                    ]
                }
            ]
        },
        "variables": {
            "dev": {
                "DBConnectionString": "dev-connectionstring"
            },
            "staging": {
                "DBConnectionString": "staging-connectionstring"
            },
            "production-account": {
                "DBConnectionString": "production-connectionstring"
            },
            "production-paypal": {
                "DBConnectionString": "#$production-account$#",
                "Version": "1.0.0",
                "PaymentAPIKey": "PaypalAPIKey"
            },
            "production-stripe": {
                "DBConnectionString": "#$production-account$#",
                "Version": "1.0.1",
                "PaymentAPIKey": "StripeAPIKey"
            }
        },
        "ignoreVariables": [
            "Version"
        ]
    }
```
The output folder structure would be as follows:
```
Sample\Generated Configs
|
|---dev_environment
|   |
|   |---Pipeline Configs
|   |  |
|   |  |---dev_Account.txt
|   |  |---dev_Payment.txt
|   |
|   |---Azure Function App Configs
|      |
|      |---dev_Account.json
|      |---dev_Payment.json
|    
|---staging_environment
|   |
|   |---Pipeline Configs
|   |  |
|   |  |---staging_Account.txt
|   |  |---staging_Payment.txt
|   |
|   |---Azure Function App Configs
|      |
|      |---staging_Account.json
|      |---staging_Payment.json
|
|---production-account_environment
|   |
|   |---Pipeline Configs
|   |  |
|   |  |---production-account_Account.txt
|   |
|   |---Azure Function App Configs
|      |
|      |---production-account_Account.json
|
|---production-paypal_environment
|   |
|   |---Pipeline Configs
|   |  |
|   |  |---production-paypal_Payment-Paypal.txt
|   |
|   |---Azure Function App Configs
|      |
|      |---production-paypal_Payment-Paypal.json
|
|---production-stripe_environment
|   |
|   |---Pipeline Configs
|   |  |
|   |  |---production-stripe_Payment-Stripe.txt
|   |
|   |---Azure Function App Configs
|      |
|      |---production-stripe_Payment-Stripe.json
```

**Sample Release Pipeline config output**
#### dev_Payment.txt (Azure DevOps Release Pipeline Configuration)

`-DBConnectionString "dev-connectionstring" -PaymentAPIKey "DevTestAPIKey"`

**Sample Azure Function App config output**
#### dev_Payment.json (Azure Function App Configuration)
```json
[
    {
        "name": "DBConnectionString",
        "value": "dev-connectionstring",
        "slotSetting": false
    },
    {
        "name": "PaymentAPIKey",
        "value": "DevTestAPIKey",
        "slotSetting": false
    }
]
```

## Building from Source
If you prefer to build the program from source using Visual Studio, please follow these steps:

1. Clone the repository or download the source code from the [GitHub repository](https://github.com/PatrickNulla/Azure-FA-Configuration-Converter).

2. Open the solution file (`Azure-FunctionApp-ConfigConverter.sln`) in Visual Studio.

3. Build the solution to generate the executable (`Azure-FunctionApp-ConfigConverter.exe`).

4. Follow the Usage instructions mentioned above to use the program.

## Downloading the Executable
If you prefer to use the pre-built executable, you can download it from the [Release](https://github.com/PatrickNulla/Azure-FA-Configuration-Converter/releases/tag/release) section of the GitHub repository.

1. Go to the [Release](https://github.com/PatrickNulla/Azure-FA-Configuration-Converter/releases/tag/release) section of the repository.

2. Download the latest release containing the executable (`Azure-FunctionApp-ConfigConverter.exe`).

3. Place the downloaded executable in the desired directory along with the `converter.json` configuration file.

4. Follow the Usage instructions mentioned above to use the program.

## Todo
- [ ] Config Generated Metadata for easy identification of generated files (e.g., When was it generated, version of the converter the config was generated, etc.).
- [ ] Add Error Handling.
- [ ] Add Unit Tests.
