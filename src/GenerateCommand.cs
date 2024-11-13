using DutchGrit.Afas.CodeGen;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DutchGrit.Afas
{
    public static class GenerateCommand
    {

        private static string DestinationFolder = "Connectors";
        private static char Separator = Path.DirectorySeparatorChar;

        public static void AddGenerateCommand(this CommandLineApplication app, ConfigFile config)
        {

            app.Command("generate", generateCmd =>
            {
                generateCmd.Description = "Generate the C# Get- and UpdateConnector files for AfasClient";


                var optionNamespace = generateCmd.Option("-n|--namespace <NAMESPACE>", "Optionally. A custom namespace during code generation.", CommandOptionType.SingleValue);

                var optionInternal = generateCmd.Option("-i|--useInternal", "Optionally. Generate internal instead of public classes.", CommandOptionType.NoValue);


                //generate will only work with a valid Afas Token.
                generateCmd.OnValidate((ctx) =>
                {
                    if ((config==null) || (!config.HasToken)) { return new ValidationResult("A valid configuration (with token) could not be found. Use [config].  "); }
                    return ValidationResult.Success;
                });


                generateCmd.OnExecuteAsync(async (cancellationToken) =>
                {
                    IAfasClient client = new AfasClient(config.MemberNumber, config.Token, (Environments)config.Environment);

                    //Make sure the folder exists.
                    System.IO.Directory.CreateDirectory(DestinationFolder);

                    //Set the emittor Options
                    var emitOptions = new EmitOptions();
                    if (optionNamespace.HasValue())
                    {
                        emitOptions.NameSpace = optionNamespace.Value();
                    }

                    if (optionInternal.HasValue())
                    {
                        emitOptions.UsePublicClass = false;
                    }

                    var si = await client.GetSessionInfoAsync();

                    if (si.GetConnectors != null) {
                        foreach (var connector in si.GetConnectors)
                        {
                            var meta = await client.GetMetaDataGetConAsync(connector.Id);
                            Console.WriteLine($"Processing : {meta.Name}");
                            SaveGetConnector(meta, emitOptions);
                        }
                    }

                    //UpdateConnector requires the use of the generator.
                    var updGenerator = new UpdateGenerator(client);
                    var updMetas = updGenerator.GenerateList();
                    foreach (var meta in updMetas)
                    {
                        Console.WriteLine($"Processing : {meta.Name}");
                        SaveUpdateConnector(meta, emitOptions);
                    }
                    

                });
            });
        }


        private static void SaveGetConnector(GetConMetaInfo meta, EmitOptions options)
        {
            var source = GetConEmit.EmitGetConnector(meta, options);
            var destFile = $"{DestinationFolder}{Separator}{Utils.StripUnderscore(meta.Name)}.cs";
            System.IO.File.WriteAllText(destFile, source);
        }

        private static void SaveUpdateConnector(UpdateConMetaInfo meta, EmitOptions options)
        {
            var source = UpdateConEmit.EmitUpdateConnector(meta,options);
            
            var destFile = $"{DestinationFolder}{Separator}{Utils.StripUnderscore(meta.Name)}.cs";
            System.IO.File.WriteAllText(destFile, source);
        }

        
    
        
      

    }
}
