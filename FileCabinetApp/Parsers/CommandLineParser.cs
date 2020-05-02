using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public static class CommandLineParser
    {
        private const char Dash = '-';
        private const string DoubleDash = "--";
        private const char Colon = ':';
        private const char Equal = '=';
        private const char WhiteSpace = ' ';
        private const string Use = "--use";

        public static Dictionary<string, string> GetCommandLineArguments(string[] args)
        {
            Dictionary<string, string> consoleParams = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                var delimeter = arg.StartsWith(Use) ? Equal : (arg.StartsWith(DoubleDash) ? Equal : Colon);
                var splitParam = arg.Split(delimeter, 2);
                consoleParams.Add(splitParam[0], splitParam[1]);
            }

            return consoleParams;
        }

        private static string JoinArguments(string[] arguments)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var argument in arguments)
            {
                stringBuilder.Append(argument);
            }

            return stringBuilder.ToString();
        }
    }
}
