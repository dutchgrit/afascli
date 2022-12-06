using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;

namespace DutchGrit.Afas
{
    public static class ValidateTokenCommand
    {

        public static void AddActivateTokenCommand(this CommandLineApplication app, ConfigFile config)
        {
            app.Command("tokenactivate", cmd =>
            {

                var optionUser = cmd.Option("-u|--user <USERID>", "Required. A Afas username (12345.username) or emailaddress.", CommandOptionType.SingleValue)
                    .IsRequired();

                var optionCode = cmd.Option("-c|--code <CODE>", "Required. The activationcode send by mail.", CommandOptionType.SingleValue)
                    .IsRequired();

                var optionSkip = cmd.Option("-s|--skip", "Option. Skip saving the received token in the config file (afas-cli.json).", CommandOptionType.NoValue);
              
                cmd.Description = "Gets a token for the specified user and its activationcode ";
                cmd.OnValidate((ctx) =>
                {
                    if ((config==null) || (!config.IsValidForOtp)) { return new ValidationResult("A valid configuration (with ApiKey and EnviromentKey) could not be found. Use [config]."); }
                    return ValidationResult.Success;
                });
                cmd.OnExecuteAsync(async (cancellationtoken) =>
                {
                    AfasOtpClient client = new AfasOtpClient(config.MemberNumber, config.ApiKey
                        , config.EnvironmentKey, (Environments)config.Environment);

                    try
                    {
                        var token = await client.GetOtpTokenValidation(optionUser.Value(), optionCode.Value());
                        Console.WriteLine(token);

                        if (!optionSkip.HasValue())
                        {
                            //store the token also in the current config
                            config.EncToken = Cryptor.Encrypt(token, Cryptor.phrase);
                            config.WriteConfig();
                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine();
                    }
                });
            });
        }
    }
}
