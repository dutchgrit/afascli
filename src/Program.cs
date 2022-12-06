using DutchGrit.Afas;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;
using System.Reflection;

namespace AfasClient.Cli
{
    class Program
    {

        public static ConfigFile config;

        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "afascli",
                FullName = "DutchGrit - Afas commandline tool"
            };

            app.VersionOptionFromAssemblyAttributes(Assembly.GetExecutingAssembly());

            

            //READ CONFIG IS AVAILABLE
            config = ConfigFile.ReadConfig();
        
            app.HelpOption(inherited: true);

            //Add commands 
            app.AddGenerateCommand(config);
            app.AddConfigCommand(config);
            app.AddInfoCommand(config);
            app.AddRequestTokenCommand(config);
            app.AddActivateTokenCommand(config);

            //INFO
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 1;
            });


            app.OnValidationError((res) =>
            {
               Console.ForegroundColor = ConsoleColor.Red;
               Console.WriteLine(res.ErrorMessage);
               Console.ResetColor();
               return 1;
           });      
            
            

            try {
                return app.Execute(args);
            }
            catch (UnrecognizedCommandParsingException ex)
            {
                Console.WriteLine($"nearest match .... {ex.NearestMatches.FirstOrDefault()}");
                return 1;
            }
            
        }
    }
    
}
