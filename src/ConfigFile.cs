using Newtonsoft.Json;
using System;

namespace DutchGrit.Afas
{
    public class ConfigFile
    {

        public static string configFileName = "afas-cli.json";
        public int MemberNumber { get; set; }
        public int Environment { get; set; }

        //Either a Token is specified directly

        public string EncToken { get; set; }


        //Or the Api en Environment keys are specified, to get a token.

        public string ApiKey { get; set; }
        public string EnvironmentKey { get; set; }

        [JsonIgnore()]
        public string Token
        {
            get
            {
                if (String.IsNullOrEmpty(EncToken)) { return ""; }
                return Cryptor.Decrypt(EncToken, Cryptor.phrase);
            }
        }

        public static ConfigFile ReadConfig()
        {
            if (!System.IO.File.Exists(configFileName))
            {
                return null;
            }
            var text = System.IO.File.ReadAllText(configFileName);
            return JsonConvert.DeserializeObject<ConfigFile>(text);
        }

        public void WriteConfig()
        {
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(configFileName, json);
        }

        public bool IsValidForOtp
        {
            get
            {
                return !(String.IsNullOrWhiteSpace(this.ApiKey) || String.IsNullOrWhiteSpace(this.EnvironmentKey));
            }
        }

        public bool HasToken
        {
            get
            {
                return !(String.IsNullOrWhiteSpace(this.Token));
            }
        }

    }
}
