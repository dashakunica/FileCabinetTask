using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// API.
    /// </summary>
    public static class Program
    {
        private const string DeveloperName = "Darya Kunickaya";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static bool isRunning = true;
        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
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
        };

        /// <summary>
        /// Entry poin of app.
        /// </summary>
        /// <param name="args">Args.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
                }
            }

            Console.WriteLine();
        }

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                try
                {
                    string firstName = EnterFirstName();
                    string lastName = EnterLastName();
                    DateTime dateOfBirth = EnterDateOfBirth();

                    var recordsCount = Program.fileCabinetService.GetStat();
                    fileCabinetService.CreateRecord(firstName, lastName, dateOfBirth);
                    Console.WriteLine($"Record #{recordsCount + 1} is created.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message + " Please try again and enter command 'create'.");
                }
            }

            Console.WriteLine();
        }

        private static void List(string parameters)
        {
            var records = Program.fileCabinetService.GetRecords();
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        private static void Edit(string parameters)
        {
            try
            {
                int id = Convert.ToInt32(parameters, NumberFormatInfo.InvariantInfo);

                string firstName = EnterFirstName();
                string lastName = EnterLastName();
                DateTime dateOfBirth = EnterDateOfBirth();

                var recordsCount = Program.fileCabinetService.GetStat();
                fileCabinetService.EditRecord(id, firstName, lastName, dateOfBirth);
                Console.WriteLine($"Record #{id} is updated.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message + "Please try again and enter command create.");
            }

            Console.WriteLine();
        }

        private static void Find(string parameter)
        {
            var inputs = parameter.Split(' ', 2);
            int indexPropertie = 0;
            int indexParameter = 1;

            FileCabinetRecord[] records;

            if (inputs[indexPropertie].Equals("firstName", StringComparison.InvariantCultureIgnoreCase))
            {
                records = fileCabinetService.FindByFirstName(inputs[indexParameter]);
                foreach (var record in records)
                {
                    Console.WriteLine(record.ToString());
                }
            }
            else if (inputs[indexPropertie].Equals("lastName", StringComparison.InvariantCultureIgnoreCase))
            {
                records = fileCabinetService.FindByLastName(inputs[indexParameter]);
                foreach (var record in records)
                {
                    Console.WriteLine(record.ToString());
                }
            }
            else if (inputs[indexPropertie].Equals("date Of birth", StringComparison.InvariantCultureIgnoreCase))
            {
                records = fileCabinetService.FindByDateOfBirth(Convert.ToDateTime(inputs[indexParameter], CultureInfo.CreateSpecificCulture("en-US")));
                foreach (var record in records)
                {
                    Console.WriteLine(record.ToString());
                }
            }
        }

        private static string EnterFirstName()
        {
            Console.Write("First name: ");
            string firstName = Console.ReadLine();
            return firstName;
        }

        private static string EnterLastName()
        {
            Console.Write("Last name: ");
            string lastName = Console.ReadLine();
            return lastName;
        }

        private static DateTime EnterDateOfBirth()
        {
            Console.Write("Date of birth: ");
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeStyles styles = DateTimeStyles.None;
            DateTime dateOfBirth = DateTime.Parse(Console.ReadLine(), culture, styles);
            return dateOfBirth;
        }
    }
}