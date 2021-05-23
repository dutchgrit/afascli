using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DutchGrit.Afas
{
    public static class InfoCommand
    {
        public static void AddInfoCommand(this CommandLineApplication app, ConfigFile config)
        {
            app.Command("info", cmd =>
            {
                cmd.Description = "Show Afas connector information.";
                cmd.OnValidate((ctx) =>
                {
                    if ((config==null) || (!config.HasToken)) { return new ValidationResult("A valid configuration (with token) could not be found. Use [config].  "); }
                    return ValidationResult.Success;
                });
                cmd.OnExecute(() =>
                {
                    IAfasClient client = new AfasClient(config.MemberNumber, config.Token, (Environments)config.Environment);

                    //try
                    //{
                    //    var version = client.GetVersion();
                    //    Console.WriteLine($"Afas version     : ", version);
                    //}
                    //catch (Exception)
                    //{
                    //    Console.WriteLine($"Afas version     : ", "AppVersion connector not installed");
                    //}

                    var info = client.GetSessionInfo();
                    Console.WriteLine($"Application Name: {info.Info.ApplicationName} ");
                    Console.WriteLine($"Environment Id: {info.Info.EnvironmentID} ");
                    Console.WriteLine($"Group: {info.Info.Group} ");


                    if (info.GetConnectors != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("GetConnectors");
                        Console.WriteLine("=============");
                        foreach (var item in info.GetConnectors)
                        {
                            Console.WriteLine($"{item.Id} - {item.Description}");
                        }
                    }

                    if (info.UpdateConnectors != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("UpdateConnectors");
                        Console.WriteLine("================");
                        foreach (var item in info.UpdateConnectors ?? new ConnectorInfo[] { })
                        {
                            Console.WriteLine($"{item.Id} - {item.Description}");
                        }
                    }

                });
            });
        }
    }
}
