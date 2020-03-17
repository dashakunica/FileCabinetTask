﻿using System;
using System.Globalization;
using System.Collections.ObjectModel;
using System.IO;

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

        private static string DefaultRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string BinaryFileName = "cabinet-records.db";

        private static string[] commandLineParameters = new string[]
        {
            "--validation-rules",
            "--storage",
        };

        private static string[] shortCommandLineParameters = new string[]
        {
            "-v",
            "-s",
        };

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService = new FileCabinetFilesystemService();
        private static IRecordValidator validator = new DefaultValidator();

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
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
            new string[] { "export", "export all records in CSV", "The 'export' command export all records in CSV"},
        };

        /// <summary>
        /// Entry poin of app.
        /// </summary>
        /// <param name="args">Args.</param>
        public static void Main(string[] args)
        {
            Console.Write("$ FileCabinetApp.exe ");
            var inputConsoleParameters = Console.ReadLine().Split(' ', 2);
            if (inputConsoleParameters[0].Equals(commandLineParameters[0], StringComparison.InvariantCultureIgnoreCase)
                || inputConsoleParameters[0].Equals(shortCommandLineParameters[0], StringComparison.InvariantCultureIgnoreCase))
            {
                string parameter = inputConsoleParameters[1];
                switch (parameter.ToLower())
                {
                    case "default":
                        validator = new DefaultValidator();
                        break;

                    case "custom":
                        validator = new CustomValidator();
                        break;
                }
            }

            if (inputConsoleParameters[0].Equals(commandLineParameters[1], StringComparison.InvariantCultureIgnoreCase)
                || inputConsoleParameters[0].Equals(shortCommandLineParameters[1], StringComparison.InvariantCultureIgnoreCase))
            {
                string parameter = inputConsoleParameters[1];
                switch (parameter.ToLower())
                {
                    case "memory":
                        fileCabinetService = new FileCabinetMemoryService();
                        Console.WriteLine("memory service");
                        break;

                    case "file":
                        string fullPath = Path.Combine(DefaultRootDirectory, BinaryFileName);
                        FileStream fileStream = File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fileCabinetService = new FileCabinetFilesystemService(fileStream);
                        Console.WriteLine("file service");
                        break;
                }
            }

            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
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
                Console.Write();
                var firstName = ReadInput(Converter.StringConverter, validator);
                Console.Write(("lastNameInputMessage", CultureInfo.InvariantCulture));
                var lastName = ReadInput(Converter.StringConverter, validator.ValidateLastName);
                Console.Write(Resource.GetString("sexInputMessage", CultureInfo.InvariantCulture));
                var sex = ReadInput(Converter.SexConverter, validator.ValidateSex);
                Console.Write(Resource.GetString("weightInputMessage", CultureInfo.InvariantCulture));
                var weight = ReadInput(Converter.WeightConverter, validator.ValidateWeight);
                Console.Write(Resource.GetString("heightInputMessage", CultureInfo.InvariantCulture));
                var height = ReadInput(Converter.HeightConverter, validator.ValidateHeight);
                Console.Write(Resource.GetString("dateOfBirthInputMessage", CultureInfo.InvariantCulture));
                DateTime dateOfBirth = ReadInput(Converter.DateOfBirthConverter, validator.ValidateDateOfBirth);
                int record = fileCabinetService.CreateRecord(height, weight, sex, firstName, lastName, dateOfBirth);
                Console.WriteLine(Resource.GetString("recordCreateMessage", CultureInfo.InvariantCulture), record);

                var record = new FileCabinetRecord
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                };

                try
                {
                    validator.ValidateParameter(record);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message + "Please try again and enter command create.");
                }

                fileCabinetService.CreateRecord(record);

                var recordsCount = Program.fileCabinetService.GetStat();
                Console.WriteLine($"Record #{recordsCount} is created.");
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

            Console.WriteLine();
        }

        private static void Edit(string parameters)
        {
            int id = Convert.ToInt32(parameters, NumberFormatInfo.InvariantInfo);

            string firstName = EnterFirstName();
            string lastName = EnterLastName();
            DateTime dateOfBirth = EnterDateOfBirth();

            var recordsCount = Program.fileCabinetService.GetStat();
            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            try
            {
                validator.ValidateParameter(record);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message + "Please try again and enter command create.");
            }

            fileCabinetService.EditRecord(record);
            Console.WriteLine($"Record #{id} is updated.");
            Console.WriteLine();
        }

        private static void Find(string parameter)
        {
            var inputs = parameter.Split(' ', 2);
            int indexPropertie = 0;
            int indexParameter = 1;

            ReadOnlyCollection<FileCabinetRecord> records;

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

            Console.WriteLine();
        }

        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<T, Tuple<bool, string>> validator)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }

        private static (string, string, DateTime, short, decimal, char) GetData()
        {
            Console.Write("First Name: ");
            var firstName = ReadInput<string>(Converter.StringConverter, dataInput.Validator.FirstNameValidator);
            Console.Write("Last Name: ");
            var lastName = ReadInput<string>(dataInput.Converter.StringConverter, dataInput.Validator.LastNameValidator);
            Console.Write("Date of birth: ");
            var dateOfBirth = ReadInput<DateTime>(dataInput.Converter.DateTimeConverter, dataInput.Validator.DateOfBirthValidator);
            Console.Write("Work place number: ");
            var workPlaceNumber = ReadInput<short>(dataInput.Converter.ShortConverter, dataInput.Validator.WorkPlaceNumberValidator);
            Console.Write("Salary: ");
            var salary = ReadInput<decimal>(dataInput.Converter.DecimalConverter, dataInput.Validator.SalaryValidator);
            Console.Write("Department: ");
            var department = ReadInput<char>(dataInput.Converter.CharConverter, dataInput.Validator.DepartmentValidator);

            return (firstName, lastName, dateOfBirth, workPlaceNumber, salary, department);
        }

        private static void Export(string parameter)
        {
            var inputs = parameter.Split(' ', 2);
            int indexPropertie = 0;
            int indexParameter = 1;

            FileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();
            if (inputs[indexPropertie].Equals("csv", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    snapshot.SaveToCsv(new StreamWriter(inputs[indexParameter], false, System.Text.Encoding.Unicode));
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Export failed: can't open file {inputs[indexParameter]}.");
                }
                finally
                {
                    Console.WriteLine($"All records are exported to file {inputs[indexParameter]}");
                }
            }

            if (inputs[indexPropertie].Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    snapshot.SaveToXml(new StreamWriter(inputs[indexParameter], false, System.Text.Encoding.Unicode));
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Export failed: can't open file {inputs[indexParameter]}.");
                }
                finally
                {
                    Console.WriteLine($"All records are exported to file {inputs[indexParameter]}");
                }
            }

        }
    }
}