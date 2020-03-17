using System;
using FileCabinetApp;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace FileCabinetGenerator
{
    class Program
    {
        private static string[] commandLineParameters = new string[]
        {
            "--output-type",
            "--output",
            "--records-amount",
            "--start-id",
        };

        private static string[] shortCommandLineParameters = new string[]
        {
            "-t",
            "-o",
            "-a",
            "-i",
        };

        static void Main(string[] args)
        {
            var parameters = GetCommandLineArguments(args);



        }

        private static Dictionary<string, string> GetCommandLineArguments(string[] args)
        {
            string arguments = JoinArguments(args);

            char delimeter = (arguments.Trim().StartsWith("-")) ? ' ' : ':';
            var dash = (arguments.Trim().StartsWith("-")) ? "-" : "--";

            var parsed = from item in arguments.Split()
                         let z = item.Split(new char[] { delimeter })
                         where z.Length >= 2 && z[0][0].Equals(dash)
                         select new KeyValuePair<string, string>(z[0], z[1]);

            Dictionary<string, string> consoleParams = new Dictionary<string, string>();
            foreach (var item in parsed)
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
