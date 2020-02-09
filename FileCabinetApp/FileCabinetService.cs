using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            if (firstName is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(firstName)} cannot be null");
            }

            if (lastName is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(lastName)} cannot be null");
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException($"Parameter {nameof(firstName)} cannot have length less than 3 or more than 60");
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException($"Parameter {nameof(lastName)} cannot have length less than 3 or more than 60");
            }

            if (firstName.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(firstName)} cannot have only whitespaces");
            }

            if (lastName.Trim().Length == 0)
            {
                throw new ArgumentException($"Parameter {nameof(lastName)} cannot have only whitespaces");
            }

            if (dateOfBirth.Day < new DateTime(1950, 01, 01).Day || dateOfBirth.Day > DateTime.Now.Day)
            {
                throw new ArgumentException($"Parameter {nameof(dateOfBirth)} cannot be less than 01-Jan-1950 and more than current day");
            }

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

            return fileCabinetRecords;
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth)
        {
            if (id > this.list.Count)
            {
                throw new ArgumentException($"This {nameof(id)} record does not exist");
            }

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            this.list[id] = record;
        }

        public FileCabinetRecord[] FindByFirstName(string firstName) { }

        public FileCabinetRecord[] FindByLastName(string firstName) { }

        public FileCabinetRecord[] FindByDateOfBirth(string firstName) { }
    }
}
