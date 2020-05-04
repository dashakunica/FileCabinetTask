using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Class which provides method for parsing command line.
    /// </summary>
    public static class CommandLineParser
    {
        private const string DoubleDash = "--";
        private const char Colon = ':';
        private const char Equal = '=';
        private const string Use = "--use";

        /// <summary>
        /// Method which get command line arguments in dictionary.
        /// </summary>
        /// <param name="args">Non parsing command line arguments.</param>
        /// <returns>Dictionary with parsing command line parameters.</returns>
        public static Dictionary<string, string> GetCommandLineArguments(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            Dictionary<string, string> consoleParams = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                var delimeter = arg.StartsWith(Use, StringComparison.OrdinalIgnoreCase) || arg.StartsWith(DoubleDash, StringComparison.OrdinalIgnoreCase)
                    ? Equal : Colon;
                var splitParam = arg.Split(delimeter, 2);
                consoleParams.Add(splitParam[0], splitParam[1]);
            }

            return consoleParams;
        }
    }
}
