using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetRecordCsvReader : IFileCabinetReader
    {
        private readonly StreamReader reader;

        public FileCabinetRecordCsvReader(StreamReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");
            }

            this.reader = reader;
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            var listOfRecords = new List<FileCabinetRecord>();
            this.reader.BaseStream.Position = 0;

            while (!this.reader.EndOfStream)
            {
                var paramaters = this.reader.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries);
                var record = CreateRecord(paramaters);

                if (record != null)
                {
                    listOfRecords.Add(record);
                }
            }

            return listOfRecords;
        }

        private static FileCabinetRecord CreateRecord(string[] paramaters)
        {
            var id = int.Parse(paramaters[0].Trim(), CultureInfo.InvariantCulture);
            var firstName = paramaters[1].Trim();
            var lastName = paramaters[2].Trim();
            var dateOfBirth = DateTime.Parse(paramaters[3].Trim(), CultureInfo.InvariantCulture);
            var workPlaceNumber = short.Parse(paramaters[4].Trim(), CultureInfo.InvariantCulture);
            var salary = decimal.Parse(paramaters[5].Trim(), CultureInfo.InvariantCulture);
            var department = char.Parse(paramaters[6].Trim());

            return new FileCabinetRecord()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Bonuses = workPlaceNumber,
                Salary = salary,
                AccountType = department,
            };
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
