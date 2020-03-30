using System;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileCabinetGenerator;

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
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
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

        private static void SetParameters(Dictionary<string, string> parameters, string[] args)
        {
            var parameterKey = (args[0].Trim().StartsWith("-")) ? shortCommandLineParameters : commandLineParameters;

            validationRules = parameters[parameterKey[0]];
            serviceRules = parameters[parameterKey[1]];

            if (validationRules.Equals(DefaultValidationRules, StringComparison.InvariantCultureIgnoreCase))
            {
                recordValidator = new ValidatorBuilder().CreateDefault();
                inputValidator = new DefaultInputValidator();
            }
            else if (opts.Rule.Equals(CustomValidationRules, StringComparison.InvariantCultureIgnoreCase))
            {
                recordValidator = new ValidatorBuilder().CreateCustom();
                inputValidator = new CustomInputValidator();
            }
            else
            {
                throw new ArgumentException(Source.Resource.GetString("invalidRule", CultureInfo.InvariantCulture));
            }

            var isFileService = serviceRules.Equals(FileServiceType, StringComparison.InvariantCultureIgnoreCase);
            fileStream = isFileService ? CreateFileStream(DataFilePath) : null;
            fileCabinetService = isFileService ? FileCabinetFilesystemService.Create(fileStream, recordValidator) : FileCabinetMemoryService.Create(recordValidator);

            var isCustomRules = o.Validation.Equals(CustomValidationRules, StringComparison.CurrentCultureIgnoreCase);
            recordValidator = isCustomRules ? ValidatorBuilder.CreateCustom() : ValidatorBuilder.CreateDefault();
        }

        private static FileStream CreateFileStream(string dataFilePath)
        {
            var fileMode = File.Exists(dataFilePath) ? FileMode.Open : FileMode.Create;
            return new FileStream(dataFilePath, fileMode, FileAccess.ReadWrite);
        }

        private static ICommandHandler CreateCommandHandler()
        {
            var helpHandler = new HelpCommandHandler();
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, Print);
            var listHandler = new ListCommandHandler(fileCabinetService, Print);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var exitHandler = new ExitCommandHandler(IsRunning);
            var createHandler = new CreateCommandHandler(inputValidator, fileCabinetService);
            var editHandler = new EditCommandHandler(inputValidator, fileCabinetService);

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
    }
}