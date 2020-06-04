using DutchGrit.Afas.CodeGen;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;

namespace DutchGrit.Afas
{
    public static class GenerateCommand
    {

        private static string DestinationFolder = "Connectors";

        public static void AddGenerateCommand(this CommandLineApplication app, ConfigFile config)
        {

            app.Command("generate", generateCmd =>
            {
                generateCmd.Description = "Generate the C# Get- and UpdateConnector files for AfasClient";

                //generate will only work with a valid Afas Token.
                generateCmd.OnValidate((ctx) =>
                {
                    if (!config.HasToken) { return new ValidationResult("A valid configuration (with token) could not be found. Use [config].  "); }
                    return ValidationResult.Success;
                });


                generateCmd.OnExecuteAsync(async (cancellationToken) =>
                {
                    IAfasClient client = new AfasClient(config.MemberNumber, config.Token, (Environments)config.Environment);

                    //Make sure the folder exists.
                    System.IO.Directory.CreateDirectory(DestinationFolder);

                    var si = await client.GetSessionInfoAsync();

                    if (si.GetConnectors != null) {
                        foreach (var connector in si.GetConnectors)
                        {
                            var meta = await client.GetMetaDataGetConAsync(connector.Id);
                            Console.WriteLine($"Processing : {meta.Name}");
                            SaveGetConnector(meta);
                        }
                    }

                    //UpdateConnector requires the use of the generator.
                    var updGenerator = new UpdateGenerator(client);
                    var updMetas = updGenerator.GenerateList();
                    foreach (var meta in updMetas)
                    {
                        Console.WriteLine($"Processing : {meta.Name}");
                        SaveUpdateConnector(meta);
                    }
                    

                });
            });
        }


        private static void SaveGetConnector(GetConMetaInfo meta)
        {
            var source = GetConEmit.EmitGetConnector(meta);
            var destFile = $"{DestinationFolder}\\{Utils.StripUnderscore(meta.Name)}.cs";
            System.IO.File.WriteAllText(destFile, source);
        }

        private static void SaveUpdateConnector(UpdateConMetaInfo meta)
        {
            var source = UpdateConEmit.EmitUpdateConnector(meta);
            var destFile = $"{DestinationFolder}\\{Utils.StripUnderscore(meta.Name)}.cs";
            System.IO.File.WriteAllText(destFile, source);
        }

        
    
        
      

    }
}
