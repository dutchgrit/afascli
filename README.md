![.NET Core](https://github.com/dutchgrit/afascli/workflows/.NET%20Core/badge.svg)
[![nuget badge](https://img.shields.io/nuget/v/Afas-Cli.svg)](https://www.nuget.org/packages/Afas-Cli/)

# afas - cli
An Afas CommandLine tool for connection info, token requests, class generation and more.
Use this tool to generate the C# typed classes, you need for the [AfasClient library](https://github.com/dutchgrit/afasclient). 


## Prerequisites

To USE the tool , you need to have the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core) or [.NET Core 6.0 SDK](https://dotnet.microsoft.com/download/dotnet-core) installed to use this tool.

> The generated code, along with the `DutchGrit.AfasClient` library, is .NET Standard 2.0.

## Installation

Run this command to install the afas-cli tool globally.
```
dotnet tool install afas-cli --global
```
 
Or update / upgrade the tool to the latest version with

```
dotnet tool update afas-cli --global
```



## Usage

### Setup connection to your Afas environment 

Open a commandprompt at the location of your project.  

```
afas-cli config
```

Provide the environment settings and token. The settings are saved in the file `afas-cli.json`. The token is stored encrypted. If you don't have a token yet, you can request one with the 'tokenrequest' and 'tokenactivate' commands. 

### Generate the GetConnector and UpdateConnector classes

To generate the classes needed for the [AfasClient library](https://github.com/dutchgrit/afasclient) , run the following command. 

```
afas-cli generate
```

Additional options are available:

```
afas-cli generate --namespace MyApplication.Connectors
```

As default, the code emitter will generate public classes for the Get- and Update Connector code.
You can opt for internal classes with the following option

```
afas-cli generate --useInternal
```


### Request a token 

If you don't have a token, you can request one. This requires that you run the config and provide it with the ApiKey and EnvironmentKey. Both key's can be found in Afas Profit in the definition of your AppConnector.

```
afas-cli tokenrequest --user 12345.yourname
```

After you receive the mail, you can activate (actual get the token) by running

```
afas-cli tokenactivate --user 12345.yourname --code <activationcode-by-mail> 
```






