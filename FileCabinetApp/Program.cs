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

        private const string CustomValidationRules = "Custom";
        private const string FileServiceType = "File";
        private const string DefaultValidationRules = "Default";
        private const string MemoryServiceType = "Memory";
        private const string LoggString = "Logging";
        private const string StopwatchString = "Stopwatch";

        private static string validationRules = string.Empty;
        private static string serviceRules = string.Empty;
        private static string loggingRules = string.Empty;

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator recordValidator;
        private static FileStream fileStream;

        private static bool isRunning = true;

        private static string DefaultRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string BinaryFileName = @"cabinet-records.db";
        private const string LoggingPath = @"log.txt";

        private static string[] commandLineParameters = new string[]
        {
            "--validation-rules",
            "--storage",
            "--use",
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
            SetParameters(commandParameters, args);

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

                var commandHandler = CreateCommandHandlers();

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
                catch (Exception)
                {
                    Console.WriteLine($"There is no explanation for '{command}' command." +
                                      $"{Environment.NewLine}The most similar commands are");

                    foreach (var item in HelpCommandHandler.Commands)
                    {
                        if (DataHelper.CalculateSimilarity(command, item) > 0.5)
                        {
                            Console.WriteLine($"\t{item}");
                        }
                    }
                }
            }
            while (isRunning);
        }

        private static void SetParameters(Dictionary<string, string> parameters, string[] args)
        {
            validationRules = DefaultValidationRules;
            serviceRules = FileServiceType;
            loggingRules = LoggString;

            if (!parameters.TryGetValue(shortCommandLineParameters[0], out validationRules))
            {
                parameters.TryGetValue(commandLineParameters[0], out validationRules);
            }

            if (!parameters.TryGetValue(shortCommandLineParameters[1], out serviceRules))
            {
                parameters.TryGetValue(commandLineParameters[1], out serviceRules);
            }

            parameters.TryGetValue(commandLineParameters[2], out loggingRules);

            var isStopwatch = true;
            var isLogger = false;
            //var isStopwatch = loggingRules.Equals(StopwatchString, StringComparison.InvariantCultureIgnoreCase);
            //var isLogger = loggingRules.Equals(LoggString, StringComparison.InvariantCultureIgnoreCase);

            SetValidators(validationRules);
            SetService(serviceRules, isStopwatch, isLogger);
        }

        private static void SetService(string serviceRules, bool isStopwatch, bool isLogger)
        {
            var isFileService = serviceRules.Equals(FileServiceType, StringComparison.InvariantCultureIgnoreCase);
            fileStream = isFileService ? CreateFileStream(BinaryFileName) : null;
            fileCabinetService = isFileService ? FileCabinetFilesystemService.Create(fileStream, recordValidator) : FileCabinetMemoryService.Create(recordValidator);
            fileCabinetService = isStopwatch ? new ServiceMeter(fileCabinetService) : fileCabinetService;
            fileCabinetService = isLogger ? new ServiceLogger(fileCabinetService, LoggingPath) : fileCabinetService;
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

        private static ICommandHandler CreateCommandHandlers()
        {
            static void Runner(bool x) => isRunning = x;

            var createHandler = new CreateCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(Runner);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var selectHandler = new SelectCommandHandler(fileCabinetService, Print);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService, Print);
            var printHelpHandler = new HelpCommandHandler();
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var updateHandler = new UpdateCommandHandler(fileCabinetService);

            createHandler.SetNext(updateHandler);
            updateHandler.SetNext(exitHandler);
            exitHandler.SetNext(exportHandler);
            exportHandler.SetNext(selectHandler);
            selectHandler.SetNext(importHandler);
            importHandler.SetNext(listHandler);
            listHandler.SetNext(printHelpHandler);
            printHelpHandler.SetNext(purgeHandler);
            purgeHandler.SetNext(deleteHandler);
            deleteHandler.SetNext(statHandler);
            statHandler.SetNext(insertHandler);

            return createHandler;
        }

        private static void Print(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }
    }
}