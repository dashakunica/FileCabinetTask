using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with file cabinet.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IRecordValidator validator;

        private readonly Dictionary<int, string> firstNameDictionary = new Dictionary<int, string>();
        private readonly Dictionary<int, string> lastNameDictionary = new Dictionary<int, string>();
        private readonly Dictionary<int, DateTime> dateOfBirthDictionary = new Dictionary<int, DateTime>();

        /// <summary>
        /// Method which create new record.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <returns>Id of record.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
            };

            this.list.Add(record);

            this.firstNameDictionary.Add(record.Id, record.FirstName);
            this.lastNameDictionary.Add(record.Id, record.LastName);
            this.dateOfBirthDictionary.Add(record.Id, record.DateOfBirth);

            return record.Id;
        }

        /// <summary>
        /// Get all records.
        /// </summary>
        /// <returns>All records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            FileCabinetRecord[] fileCabinetRecords = new FileCabinetRecord[this.list.Count];
            for (int i = 0; i < this.list.Count; i++)
            {
                fileCabinetRecords[i] = this.list[i];
            }

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(fileCabinetRecords);

            return readOnlyRecords;
        }

        /// <summary>
        /// Get amount of record.
        /// </summary>
        /// <returns>Amount of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Edit current record.
        /// </summary>
        /// <param name="record">Record.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record.Id > this.list.Count)
            {
                throw new ArgumentException($"This {nameof(record.Id)} record does not exist.");
            }

            this.list[record.Id - 1] = record;

            this.firstNameDictionary[record.Id - 1] = record.FirstName;
            this.lastNameDictionary[record.Id - 1] = record.LastName;
            this.dateOfBirthDictionary[record.Id - 1] = record.DateOfBirth;
        }

        /// <summary>
        /// Methods for finding first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Records with this first name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase);
                }).ToArray();

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(fileCabinetRecords);

            return readOnlyRecords;
        }

        /// <summary>
        /// Methods for finding last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Records with this last name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase);
                }).ToArray();

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(fileCabinetRecords);

            return readOnlyRecords;
        }

        /// <summary>
        /// Methods for finding date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Record with this date of birth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.DateOfBirth.Equals(dateOfBirth);
                }).ToArray();

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(fileCabinetRecords);

            return readOnlyRecords;
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileCabinetRecord[] fileCabinetRecords = new FileCabinetRecord[this.list.Count];
            for (int i = 0; i < this.list.Count; i++)
            {
                fileCabinetRecords[i] = this.list[i];
            }

            FileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot(fileCabinetRecords);

            return snapshot;
        }
    }
}
