using System;
using System.Collections.Generic;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Help command.
    /// </summary>
    public class HelpCommandHandler : CommandHandlerBase
    {
        private const string Command = "help";

        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "shows amount of records", "The 'stat' command prints amount of records." },
            new string[] { "create", "create new records", "The 'create' command create new records." },
            new string[]
            {
                "select", "select records", $"The 'select' command select and show specified fields of records using clause in table format." +
                 $"{Environment.NewLine}Example: select id, firstname, lastname where firstname = 'John' and lastname = 'Doe'.",
            },
            new string[]
            {
                "update", "update records", $"The 'update' command update records using clause." +
                $"{Environment.NewLine}Example: update set firstname = 'John', lastname = 'Doe' , dateofbirth = '5/18/1986' where id = '1'",
            },
            new string[]
            {
                "delete", "delete records", "The 'delete' command delete records using clause." +
                $"{Environment.NewLine}Example: delete where id = '1'",
            },
            new string[]
            {
                "insert", "insert records", "The 'insert' command insert specific field info in record." +
                 $"{Environment.NewLine}Example: insert (id, firstname, lastname, dateofbirth) values ('1', 'John', 'Doe', '5/18/1986')",
            },
            new string[] { "export", "export all records in CSV or XML", "The 'export' command export all records in CSV or XML" },
            new string[] { "import", "import records", "The 'import' command import records from csv or xml file." },
            new string[] { "purge", "clear filesystem", "The 'purge' command clear filesystem. Use only in FileCabinetFilesystemService." },
            new string[] { "list", "get all records", "The 'list' command get all records in table format." },
        };

        /// <summary>
        /// Gets all commands.
        /// </summary>
        /// <value>
        /// All commands string.
        /// </value>
        public static IEnumerable<string> Commands => HelpMessages.Select(x => x[0]);

        /// <inheritdoc/>
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
