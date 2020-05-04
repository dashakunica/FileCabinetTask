using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    class Program
    {
        private const string XmlString = "xml";
        private const string CsvString = "csv";

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
            var parameters = CommandLineParser.GetCommandLineArguments(args);

            var parameterKey = (args[0].Trim().StartsWith("--")) ? commandLineParameters : shortCommandLineParameters;

            string fileType = parameters[parameterKey[0]];
            string filePath = parameters[parameterKey[1]];
            int recordsAmount = Int32.Parse(parameters[parameterKey[2]]);
            int startId = Int32.Parse(parameters[parameterKey[3]]);

            if (!fileType.Equals(CsvString, StringComparison.InvariantCultureIgnoreCase) &&
                !fileType.Equals(XmlString, StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("You can only export records into *.csv or *.xml format.");
                return;
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var stream = File.Create(filePath);
            using var writer = new StreamWriter(stream);
            var records = Generate(startId, recordsAmount);

            if (fileType.Equals(CsvString, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var record in records)
                {
                    writer.WriteLine(record.ToString());
                }
            }

            if (fileType.Equals(XmlString, StringComparison.CurrentCultureIgnoreCase))
            {
                var xmlSerializedRecords = new FileCabinetRecordsSerializable(records);

                var ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                var serializer = new XmlSerializer(typeof(FileCabinetRecordsSerializable));
                serializer.Serialize(writer, xmlSerializedRecords, ns);
            }

            Console.WriteLine($"{recordsAmount} records were written to {filePath}");
        }

        private static IEnumerable<FileCabinetRecordSerializable> Generate(int startId, int amount)
        {
            var validation = ValidatorBuilder.CreateDefault();
            int id = 0;

            var random = new Random();
            for (int i = startId; i <= startId + amount; i++)
            {
                var data = new ValidateParametersData();
                bool isValid;

                do
                {
                    try
                    {
                        id = i;

                        var day = random.Next(1, 30);
                        var month = random.Next(1, 12);
                        var year = random.Next(1950, 2020);
                        var date = new DateTime(year, month, day);

                        data = new ValidateParametersData
                        {
                            FirstName = RandomString(random.Next(2, 60)),
                            LastName = RandomString(random.Next(2, 60)),
                            DateOfBirth = date,
                            Bonuses = Convert.ToInt16(random.Next(0, 30_000)),
                            Salary = random.Next(3_000, 10_000),
                            AccountType = (char)random.Next('a', 'z'),
                        };

                        validation.ValidateParameters(data);
                        isValid = true;
                    }
                    catch (ArgumentException)
                    {
                        isValid = false;
                    }
                }
                while (!isValid);

                yield return new FileCabinetRecordSerializable
                {
                    Id = id,
                    Name = new Name() 
                    { 
                        FirstName = data.FirstName,
                        LastName = data.LastName
                    },
                    DateOfBirth = data.DateOfBirth,
                    Bonuses = data.Bonuses,
                    Salary = data.Salary,
                    AccountType = data.AccountType,
                }; 
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
