using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] fileCabinetRecords = new FileCabinetRecord[this.list.Count];
            for (int i = 0; i < this.list.Count; i++)
            {
                fileCabinetRecords[i] = this.list[i];
            }

            return Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
