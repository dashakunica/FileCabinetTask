using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for starting initialization application.
    /// </summary>
    public static class Startup
    {
        private const string CustomValidationRules = "custom";
        private const string DefaultValidationRules = "default";
        private const string FileServiceType = "file";

        private const string ValidationFileName = @"validation-rules.json";
        private const string BinaryFileName = @"cabinet-records.db";
        private const string LoggingPath = @"log.txt";

        private static readonly string DefaultRootDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static IFileCabinetService fileCabinetService;
        private static IRecordValidator recordValidator;
        private static FileStream fileStream;

        static Startup()
        {
            if (!File.Exists(ValidationPath))
            {
                Console.WriteLine($"Missing json file {ValidationFileName}.");
                Environment.Exit(1);
            }

            Configuration = new ConfigurationBuilder().AddJsonFile(ValidationPath).Build();
        }

        /// <summary>
        /// Gets or sets configuration type.
        /// </summary>
        /// <value>
        /// Configuration type.
        /// </value>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets validation path.
        /// </summary>
        /// <value>
        /// Validation path.
        /// </value>
        public static string ValidationPath { get; set; } = Path.Combine(DefaultRootDirectory, ValidationFileName);

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        /// <value>
        /// Is running.
        /// </value>
        public static bool IsRunning { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether is file sevice.
        /// </summary>
        /// <value>
        /// Is file service.
        /// </value>
        public static bool IsFileService { get; private set; }

        /// <summary>
        /// Gets validation type.
        /// </summary>
        /// <value>
        /// Validation type.
        /// </value>
        public static string ValidationType { get; private set; } = DefaultValidationRules;

        /// <summary>
        /// Run and initialize all servecies and parameters.
        /// </summary>
        /// <param name="args">Command line argument.</param>
        public static void Run(string[] args)
        {
            var consoleParametersDictionary = CommandLineParser.GetCommandLineArguments(args);
            var (validationRules, serviceRules, isStopwatch, isLogger) = ParameterParser.SetParameters(consoleParametersDictionary);
            SetValidators(validationRules);
            SetService(serviceRules, isStopwatch, isLogger);
            Console.WriteLine($"Using {serviceRules} service and {validationRules} validation rules.");
        }

        /// <summary>
        /// Create command handler.
        /// </summary>
        /// <returns>Command handler.</returns>
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

        private static void SetService(string serviceRules, bool isStopwatch, bool isLogger)
        {
            var isFileService = serviceRules.Equals(FileServiceType, StringComparison.InvariantCultureIgnoreCase);

            fileStream = isFileService ? CreateFileStream(BinaryFileName) : null;

            fileCabinetService = isFileService
                ? FileCabinetFilesystemService.Create(fileStream, recordValidator)
                : FileCabinetMemoryService.Create(recordValidator);

            IsFileService = isFileService;

            fileCabinetService = isStopwatch ? new ServiceMeter(fileCabinetService) : fileCabinetService;

            fileCabinetService = isLogger ? new ServiceLogger(fileCabinetService, LoggingPath) : fileCabinetService;
        }

        private static void SetValidators(string validationRules)
        {
            if (validationRules.Equals(CustomValidationRules, StringComparison.CurrentCultureIgnoreCase) ||
                validationRules.Equals(DefaultValidationRules, StringComparison.CurrentCultureIgnoreCase))
            {
                ValidationType = validationRules;
            }
            else
            {
                Console.WriteLine("Validation type does not recognazed. Use default rules.");
            }

            var isCustomRules = validationRules.Equals(CustomValidationRules, StringComparison.CurrentCultureIgnoreCase);
            recordValidator = isCustomRules ? ValidatorBuilder.CreateCustom() : ValidatorBuilder.CreateDefault();
        }

        private static FileStream CreateFileStream(string dataFilePath)
        {
            var path = Path.Combine(DefaultRootDirectory, dataFilePath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return new FileStream(dataFilePath, FileMode.Create, FileAccess.ReadWrite);
        }

        private static void Print(IEnumerable<FileCabinetRecord> records, List<string> properties)
        {
            var printer = new TablePrinter<FileCabinetRecord>();
            printer.ToTableFormat(records, properties);
        }
    }
}
