using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public static class ParameterParser
    {
        private const string FileServiceType = "File";
        private const string DefaultValidationRules = "Default";
        private const string LoggString = "Logging";
        private const string StopwatchString = "Stopwatch";

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

        public static (string validationRules, string serviceRules, bool isStopwatch, bool isLogger) SetParameters(Dictionary<string, string> parameters)
        {
            if (parameters is null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var validationRules = DefaultValidationRules;
            var serviceRules = FileServiceType;
            var loggingRules = LoggString;

            if (!parameters.TryGetValue(shortCommandLineParameters[0], out validationRules))
            {
                parameters.TryGetValue(commandLineParameters[0], out validationRules);
            }

            if (!parameters.TryGetValue(shortCommandLineParameters[1], out serviceRules))
            {
                parameters.TryGetValue(commandLineParameters[1], out serviceRules);
            }

            parameters.TryGetValue(commandLineParameters[2], out loggingRules);

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
