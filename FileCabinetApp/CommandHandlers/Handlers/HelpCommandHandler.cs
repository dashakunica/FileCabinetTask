using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string Command = "help";

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows amount of records", "The 'stat' command prints amount of records." },
            new string[] { "create", "create new records", "The 'create' command create new records." },
            new string[] { "list", "shows list of records", "The 'list' command shows list of records." },
            new string[] { "edit", "edit current records", "The 'edit' command edit current record." },
            new string[] { "find firstname", "find all records with current firstname", "The 'find firstname' command shows all records with current firstname." },
            new string[] { "find lastname", "find all records with current lastname", "The 'find lastname' command shows all records with current lastname." },
            new string[] { "find dateofbirth", "find all records with current dateofbirth", "The 'find dateofbirth' command shows all records with current dateofbirth." },
            new string[] { "export", "export all records in CSV", "The 'export' command export all records in CSV"},
            new string[] { "import", "import records", "The 'import' import records from file." },
            new string[] { "remove", "remove record", "The 'remove' remove specified record." },
        };

        public static IEnumerable<string> Commands => HelpMessages.Select(x => x[0]);

        public override void Handle(AppCommandRequest commandRequest)
        {
            if (commandRequest is null)
            {
                throw new ArgumentNullException(nameof(commandRequest));
            }

            if (Command.Equals(commandRequest?.Command, StringComparison.InvariantCultureIgnoreCase))
            {
                PrintHelp(commandRequest?.Parameters);
            }
            else
            {
                base.Handle(commandRequest);
            }
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                int index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"This command'{parameters}' does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine($"\t{helpMessage[CommandHelpIndex]}\t - {helpMessage[DescriptionHelpIndex]}");
                }
            }

            Console.WriteLine();
        }
    }
}
