using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

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
            var parameters = CommandLineParser.GetCommandLineArguments(args);

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

            using var stream = File.Create(filePath);
            using var writer = new StreamWriter(stream);
            var records = Generate(startId, recordsAmount);

            if (fileType.Equals("csv", StringComparison.CurrentCultureIgnoreCase))
            {
                var fileWriter = new StreamWriter(filePath);
                var csvWriter = new CsvWriter(fileWriter, records);
                csvWriter.Write();
                fileWriter.Close();
            }

            if (fileType.Equals("xml", StringComparison.CurrentCultureIgnoreCase))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true
                };
                using (var fileWriter = XmlWriter.Create(filePath, settings))
                {
                    var xmlWriter = new XmlWriters(fileWriter, records.ToArray());
                    xmlWriter.Write();
                }
            }
        }

        private static void ExportAsCsv(IEnumerable<FileCabinetRecordSerializable> records, StreamWriter writer)
        {
            foreach (var record in records)
            {
                writer.WriteLine(record.ToString());
            }
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

                        validation.ValidateParameters(record);
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
