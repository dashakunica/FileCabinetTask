using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    public static class Startup
    {
        private static string DefaultRootDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string ValidationFileName = @"validation-rules.json";

        private const string CustomValidationRules = "custom";
        private const string FileServiceType = "file";

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator recordValidator;
        private static FileStream fileStream;

        private static string BinaryFileName = @"cabinet-records.db";
        private static string LoggingPath = @"log.txt";

        static Startup()
        {
            if (!File.Exists(ValidationPath))
            {
                Console.WriteLine($"Missing json file {ValidationFileName}.");
                Environment.Exit(1488);
            }

            Configuration = new ConfigurationBuilder().AddJsonFile(ValidationPath).Build();
        }

        public static IConfiguration Configuration { get; set; }

        public static string ValidationPath { get; set; } = Path.Combine(DefaultRootDirectory, ValidationFileName);

        public static bool IsRunning { get; private set; } = true;

        public static void Run(string[] args)
        {
            var consoleParametersDictionary = CommandLineParser.GetCommandLineArguments(args);
            var parameters = ParameterParser.SetParameters(consoleParametersDictionary);
            SetValidators(parameters.validationRules);
            SetService(parameters.serviceRules, parameters.isStopwatch, parameters.isLogger);
            Console.WriteLine($"Using {parameters.serviceRules} service and {parameters.validationRules} validation rules.");
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

        public static ICommandHandler CreateCommandHandlers()
        {
            static void Runner(bool x) => IsRunning = x;

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
            var printer = new TablePrinter<FileCabinetRecord>();
            printer.ToTableFormat(records);
        }
    }
}
