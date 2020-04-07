using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public static class CommandLineParser
    {
        private const string Dash = "-";
        private const string DoubleDash = "--";
        private const char WhiteSpace = ' ';
        private const char Colon = ':';

        public static Dictionary<string, string> GetCommandLineArguments(string[] args)
        {
            string arguments = JoinArguments(args);

            char delimeter = arguments.Trim().StartsWith(Dash) ? WhiteSpace : Colon;
            string dash = arguments.Trim().StartsWith(Dash) ? Dash : DoubleDash;

            var parsedPair = from item in arguments.Split()
                         let z = item.Split(delimeter)
                         where z.Length >= 2 && z[0][0].Equals(dash)
                         select new KeyValuePair<string, string>(z[0], z[1]);

            var parsedParam = from item in arguments.Split()
                         let z = item.Split(delimeter)
                         where z.Length == 1 && z[0][0].Equals(dash)
                         select new KeyValuePair<string, string>(z[0], "default");

            Dictionary<string, string> consoleParams = new Dictionary<string, string>();
            foreach (var item in parsedPair)
            {
                consoleParams.Add(item.Key, item.Value);
            }

            foreach (var item in parsedParam)
            {
                consoleParams.Add(item.Key, item.Value);
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
