using System;
using System.Collections.Generic;
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

        private const string CustomValidationRules = "Custom";
        private const string FileServiceType = "File";
        private const string DefaultValidationRules = "Default";
        private const string MemoryServiceType = "Memory";

        private static string validationRules = string.Empty;
        private static string serviceRules = string.Empty;

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator recordValidator;
        private static IInputValidator inputValidator;
        private static FileStream fileStream;

        private static bool isRunning = true;

        private static string DefaultRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string BinaryFileName = @"cabinet-records.db";

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

        /// <summary>
        /// Entry poin of app.
        /// </summary>
        /// <param name="args">Args.</param>
        public static void Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException($"{nameof(args)} cannot be null.");
            }

            var commandParameters = CommandLineParser.GetCommandLineArguments(args);

            Console.WriteLine($"Using {validationRules} validation rules.");
            Console.WriteLine($"File Cabinet Application, developed by {DeveloperName}");
            Console.WriteLine(HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                var commandHandler = CreateCommandHandler();

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                const int parametersIndex = 1;
                string parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                try
                {
                    commandHandler.Handle(new AppCommandRequest(command, parameters));
                }
                catch (NullReferenceException)
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        private static void SetParameters(Dictionary<string, string> parameters, string[] args)
        {
            var parameterKey = (args[0].Trim().StartsWith("-")) ? shortCommandLineParameters : commandLineParameters;

            validationRules = parameters[parameterKey[0]];
            serviceRules = parameters[parameterKey[1]];

            SetValidators(validationRules);
            SetService(serviceRules);
        }

        private static void SetService(string serviceRules)
        {
            var isFileService = serviceRules.Equals(FileServiceType, StringComparison.InvariantCultureIgnoreCase);
            fileStream = isFileService ? CreateFileStream(BinaryFileName) : null;
            fileCabinetService = isFileService ? FileCabinetFilesystemService.Create(fileStream, recordValidator) : FileCabinetMemoryService.Create(recordValidator);
        }

        private static void SetValidators(string validationRules)
        {
            var isCustomRules = validationRules.Equals(CustomValidationRules, StringComparison.CurrentCultureIgnoreCase);
            recordValidator = isCustomRules ? ValidatorBuilder.CreateCustom() : ValidatorBuilder.CreateDefault();
        }

        private static FileStream CreateFileStream(string dataFilePath)
        {
            var path = Path.Combine(DefaultRootDirectory, dataFilePath);
            var fileMode = File.Exists(path) ? FileMode.Open : FileMode.Create;
            return new FileStream(dataFilePath, fileMode, FileAccess.ReadWrite);
        }

        private static ICommandHandler CreateCommandHandler()
        {
            static void Runner(bool x) => isRunning = x;
            static void Printer(IEnumerable<FileCabinetRecord> x) => Print(x);

            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, Printer);
            var listHandler = new ListCommandHandler(fileCabinetService, Printer);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(Runner);
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);

            helpHandler.SetNext(importHandler).SetNext(exportHandler).
                SetNext(findHandler).SetNext(listHandler).SetNext(purgeHandler).
                SetNext(removeHandler).SetNext(statHandler).SetNext(exitHandler).
                SetNext(createHandler).SetNext(editHandler);

            return helpHandler;
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        private static void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                Console.WriteLine($"#{record.ToString()}");
            }
        }
    }
}