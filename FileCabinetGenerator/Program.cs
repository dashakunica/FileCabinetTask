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

            var parameterKey = (args[0].Trim().StartsWith("-")) ? shortCommandLineParameters : commandLineParameters;

            string fileType = parameters[parameterKey[0]];
            string filePath = parameters[parameterKey[1]];
            int recordsAmount = Int32.Parse(parameters[parameterKey[2]]);
            int startId = Int32.Parse(parameters[parameterKey[3]]);

            if (!fileType.Equals("csv", StringComparison.InvariantCultureIgnoreCase) && 
                !fileType.Equals("xml", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("U can only export records into *.csv or *.xml format.");
                return;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
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

        private static IEnumerable<FileCabinetRecordSerializable> Generate(int startId, int amount)
        {
            var validation = new DefaultValidator();

            var list = new List<FileCabinetRecordSerializable>();
            var random = new Random();
            for (int i = 0; i < amount; i++)
            {
                var record = new FileCabinetRecordSerializable();
                bool isValid;

                do
                {
                    try
                    {
                        record.Id = startId;
                        startId++;

                        record.Name.FirstName = RandomString(random.Next(4, 60));
                        record.Name.LastName = RandomString(random.Next(4, 60));

                        var day = random.Next(1, 30);
                        var month = random.Next(1, 12);
                        var year = random.Next(1950, 2020);
                        record.DateOfBirth = record.DateOfBirth.AddDays(day - 1);
                        record.DateOfBirth = record.DateOfBirth.AddMonths(month - 1);
                        record.DateOfBirth = record.DateOfBirth.AddYears(year - 1);

                        record.AccountType = (char)random.Next(64, 100);
                        record.Salary = random.Next(30, 200);
                        record.Bonuses = Convert.ToInt16(random.Next(120, 240));

                        validation.ValidateParameters((record.Name.FirstName, record.Name.LastName, record.DateOfBirth, record.Bonuses, record.Salary, record.AccountType));
                        isValid = true;
                    }
                    catch
                    {
                        isValid = false;
                    }
                }
                while (!isValid);

                yield return record;
            }
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
