using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Parameter parser.
    /// </summary>
    public static class ParameterParser
    {
        private const string FileServiceType = "File";
        private const string DefaultValidationRules = "Default";
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

            var validationRules = DefaultValidationRules;
            var serviceRules = FileServiceType;
            var loggingRules = LoggString;

            if (!parameters.TryGetValue(ShortCommandLineParameters[0], out validationRules))
            {
                parameters.TryGetValue(CommandLineParameters[0], out validationRules);
            }

            if (!parameters.TryGetValue(ShortCommandLineParameters[1], out serviceRules))
            {
                parameters.TryGetValue(CommandLineParameters[1], out serviceRules);
            }

            parameters.TryGetValue(CommandLineParameters[2], out loggingRules);

            bool isStopwatch = loggingRules.Equals(StopwatchString, StringComparison.InvariantCultureIgnoreCase);
            bool isLogger = loggingRules.Equals(LoggString, StringComparison.InvariantCultureIgnoreCase);

            if (validationRules is null || serviceRules is null)
            {
                validationRules = DefaultValidationRules;
                serviceRules = FileServiceType;
            }

            return (validationRules, serviceRules, isStopwatch, isLogger);
        }
    }
}
