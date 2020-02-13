using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with file cabinet.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<int, string> firstNameDictionary = new Dictionary<int, string>();
        private readonly Dictionary<int, string> lastNameDictionary = new Dictionary<int, string>();
        private readonly Dictionary<int, DateTime> dateOfBirthDictionary = new Dictionary<int, DateTime>();

        /// <summary>
        /// Method which create new record.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Id of record.</returns>
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

            this.firstNameDictionary.Add(record.Id, record.FirstName);
            this.lastNameDictionary.Add(record.Id, record.LastName);
            this.dateOfBirthDictionary.Add(record.Id, record.DateOfBirth);

            return record.Id;
        }

        /// <summary>
        /// Get all records.
        /// </summary>
        /// <returns>All records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] fileCabinetRecords = new FileCabinetRecord[this.list.Count];
            for (int i = 0; i < this.list.Count; i++)
            {
                fileCabinetRecords[i] = this.list[i];
            }

            return fileCabinetRecords;
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
        /// <param name="id">Id.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="dateOfBirth">Date of birth.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth)
        {
            if (id > this.list.Count)
            {
                throw new ArgumentException($"This {nameof(id)} record does not exist.");
            }

            var record = new FileCabinetRecord
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            this.list[id - 1] = record;

            this.firstNameDictionary[id - 1] = record.FirstName;
            this.lastNameDictionary[id - 1] = record.LastName;
            this.dateOfBirthDictionary[id - 1] = record.DateOfBirth;
        }

        /// <summary>
        /// Methods for finding first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Records with this first name.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase);
                }).ToArray();

            return fileCabinetRecords;
        }

        /// <summary>
        /// Methods for finding last name.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <returns>Records with this last name.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase);
                }).ToArray();

            return fileCabinetRecords;
        }

        /// <summary>
        /// Methods for finding date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        /// <returns>Record with this date of birth.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime dateOfBirth)
        {
            FileCabinetRecord[] fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.DateOfBirth.Equals(dateOfBirth);
                }).ToArray();

            return fileCabinetRecords;
        }
    }
}
