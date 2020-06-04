using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;

namespace DutchGrit.Afas
{
    public static class RequestTokenCommand
    {

        public static void AddRequestTokenCommand(this CommandLineApplication app, ConfigFile config)
        {
            app.Command("tokenrequest", cmd =>
            {

                var optionUser = cmd.Option("-u|--user <USERID>", "Required. A Afas username (12345.username) or emailaddress.", CommandOptionType.SingleValue)
                    .IsRequired();

                cmd.Description = "Request a token for a specified user.";
                cmd.OnValidate((ctx) =>
                {
                    if (!config.IsValidForOtp) { return new ValidationResult("A valid configuration (with ApiKey and EnviromentKey) could not be found. Use [config]."); }
                    return ValidationResult.Success;
                });

                cmd.OnExecuteAsync(async (cancellationtoken) =>
                {
                    OtpTokenClient client = new OtpTokenClient(config.MemberNumber, config.ApiKey
                        , config.EnvironmentKey, (Environments)config.Environment);
                   
                    var x = await client.GetOtpTokenRequest(optionUser.Value());
                });
            });
        }
    }
}
