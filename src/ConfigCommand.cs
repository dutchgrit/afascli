using McMaster.Extensions.CommandLineUtils;
using System.Collections.Generic;
using System.Text;

namespace DutchGrit.Afas
{

    public static class ConfigCommand
    {
        public static void AddConfigCommand(this CommandLineApplication app, ConfigFile config)
        {
            app.Command("config", cmd =>
            {
                cmd.Description = "Specify the (local) configuration used for afas-cli.";
                cmd.OnExecute(() =>
               {
                  
                   if (config == null) { config = new ConfigFile { MemberNumber = 12345 }; }

                   config.MemberNumber = Prompt.GetInt("Afas membernumber : ", config?.MemberNumber);
                   config.Environment = Prompt.GetInt("Environment (0=Production,1=Test,2=Acceptance) : ", config?.Environment);
                   var token = Prompt.GetString("Token (Skip if you need to request a token) : ");
                   
                   config.EncToken = Cryptor.Encrypt(token, Cryptor.phrase);
                   config.ApiKey = Prompt.GetString("ApiKey (Skip if you already have a token) : ", config?.ApiKey);
                   config.EnvironmentKey = Prompt.GetString("EnvironmentKey (Skip if you already have a token : ", config?.EnvironmentKey);

                   config.WriteConfig();
               });
            });
        }
    }
}
