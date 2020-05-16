using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Parameter parser.
    /// </summary>
    public static class ParameterParser
    {
        private const string MemoryServiceType = "memory";
        private const string DefaultValidationRules = "default";
        private const string FileServiceType = "file";
        private const string CustomValidationRules = "custom";
        private const string LoggString = "Logging";
        private const string StopwatchString = "Stopwatch";

        private static readonly string[] CommandLineParameters = new string[]
        {
            "--validation-rules",
            "--storage",
            "--use",
        };

        private static readonly string[] ShortCommandLineParameters = new string[]
        {
            "-v",
            "-s",
        };

        /// <summary>
        /// Set parameters from dictionary.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        /// <returns>Parsed parameters.</returns>
        public static (string validationRules, string serviceRules, bool isStopwatch, bool isLogger) SetParameters(Dictionary<string, string> parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!parameters.TryGetValue(ShortCommandLineParameters[0], out string validationRules))
            {
                parameters.TryGetValue(CommandLineParameters[0], out validationRules);
            }

            if (!parameters.TryGetValue(ShortCommandLineParameters[1], out string serviceRules))
            {
                parameters.TryGetValue(CommandLineParameters[1], out serviceRules);
            }

            parameters.TryGetValue(CommandLineParameters[2], out string loggingRules);

            if (loggingRules is null)
            {
                loggingRules = string.Empty;
            }

            bool isStopwatch = loggingRules.Equals(StopwatchString, StringComparison.InvariantCultureIgnoreCase);
            bool isLogger = loggingRules.Equals(LoggString, StringComparison.InvariantCultureIgnoreCase);

            if (validationRules is null)
            {
                validationRules = DefaultValidationRules;
            }

            if (serviceRules is null)
            {
                serviceRules = MemoryServiceType;
            }

            if (!validationRules.Equals(DefaultValidationRules, StringComparison.InvariantCultureIgnoreCase)
                && !validationRules.Equals(CustomValidationRules, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Unknown validation rules. Using default settings.");
                validationRules = DefaultValidationRules;
            }

            if (!serviceRules.Equals(MemoryServiceType, StringComparison.InvariantCultureIgnoreCase)
                && !serviceRules.Equals(FileServiceType, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Unknown service type. Using default settings.");
                serviceRules = MemoryServiceType;
            }

            return (validationRules, serviceRules, isStopwatch, isLogger);
        }
    }
}
