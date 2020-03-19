using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with file cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IRecordValidator validator;

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        /// <summary>
        /// Method which create new record.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <returns>Id of record.</returns>
        public int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            return this.CreateRecordWithSpecifiedId(null, data);
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
        public (int active, int removed) GetStat() => (this.list.Count, 0);

        public void Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Edit current record.
        /// </summary>
        /// <param name="record">Record.</param>
        public void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            var editedRecord = this.list.First(x => x.Id == id);

            var resentFirstName = editedRecord.FirstName;
            var resentLastName = editedRecord.LastName;
            var resentDateOfBirth = editedRecord.DateOfBirth;

            editedRecord.FirstName = data.firstName;
            editedRecord.LastName = data.lastName;
            editedRecord.DateOfBirth = data.dateOfBirth;
            editedRecord.Bonuses = data.bonuses;
            editedRecord.Salary = data.salary;
            editedRecord.AccountType = data.accountType;

            this.UpdateRecordInFirstNameDictionary(data.firstName, editedRecord, resentFirstName);
            this.UpdateRecordInLastNameDictionary(data.lastName, editedRecord, resentLastName);
            this.UpdateRecordInDateOfBirthDictionary(data.dateOfBirth, editedRecord, resentDateOfBirth);
        }

        /// <summary>
        /// Methods for finding first name.
        /// </summary>
        /// <param name="firstName">First name.</param>
        /// <returns>Records with this first name.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase);
                });

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
            var fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase);
                });

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
            var fileCabinetRecords = this.list.FindAll(
                delegate(FileCabinetRecord name)
                {
                    return name.DateOfBirth.Equals(dateOfBirth);
                });

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

        public void Restore(FileCabinetServiceSnapshot snapshot, out int failed)
        {
            failed = 0;

            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} cannot be null.");
            }

            foreach (var record in snapshot.Records)
            {
                try
                {
                    if (this.GetRecords().Any(x => x.Id == record.Id))
                    {
                        this.EditRecord(record.Id, (record.FirstName, record.LastName, record.DateOfBirth, record.Bonuses, record.Salary, record.AccountType));
                    }
                    else
                    {
                        this.CreateRecordWithSpecifiedId(record.Id, (record.FirstName, record.LastName, record.DateOfBirth, record.Bonuses, record.Salary, record.AccountType));
                    }
                }
                catch (IndexOutOfRangeException ioor)
                {
                    ++failed;
                    Console.WriteLine($"Import record with id {record.Id} failed: {ioor.Message}");
                }
                catch (ArgumentException ae)
                {
                    ++failed;
                    Console.WriteLine($"Import record with id {record.Id} failed: {ae.Message}");
                }
            }
        }

        private int CreateRecordWithSpecifiedId(int? id, (string fName, string lName, DateTime dob, short wpn, decimal salary, char department) data)
        {
            if (id.HasValue)
            {
                if (id.Value < 0)
                {
                    throw new IndexOutOfRangeException($"{id} cannot be less then 1.");
                }
            }

            var record = new FileCabinetRecord()
            {
                Id = id != null ? id.Value : this.list.Count + 1,
                FirstName = data.fName,
                LastName = data.lName,
                DateOfBirth = data.dob,
                Bonuses = data.wpn,
                Salary = data.salary,
                AccountType = data.department,
            };

            this.list.Add(record);
            ServiceHelper.AddRecordToDictionary(record.FirstName, record, this.firstNameDictionary);
            ServiceHelper.AddRecordToDictionary(record.LastName, record, this.lastNameDictionary);
            ServiceHelper.AddRecordToDictionary(record.DateOfBirth, record, this.dateOfBirthDictionary);

            return record.Id;
        }

        private void UpdateRecordInFirstNameDictionary(string newFirstName, FileCabinetRecord record, string resentFirstName)
        {
            FileCabinetRecord updateItem = this.firstNameDictionary[resentFirstName].First(x => x.Id == record.Id);

            this.firstNameDictionary[resentFirstName].Remove(updateItem);

            if (!this.firstNameDictionary.ContainsKey(newFirstName))
            {
                this.firstNameDictionary.Add(newFirstName, new List<FileCabinetRecord>());
            }

            this.firstNameDictionary[newFirstName].Add(record);
        }

        private void UpdateRecordInLastNameDictionary(string newLastName, FileCabinetRecord record, string resentLastName)
        {
            FileCabinetRecord updateItem = this.lastNameDictionary[resentLastName].First(x => x.Id == record.Id);

            this.lastNameDictionary[resentLastName].Remove(updateItem);

            if (!this.lastNameDictionary.ContainsKey(newLastName))
            {
                this.lastNameDictionary.Add(newLastName, new List<FileCabinetRecord>());
            }

            this.lastNameDictionary[newLastName].Add(record);
        }

        private void UpdateRecordInDateOfBirthDictionary(DateTime newDateOfBirth, FileCabinetRecord record, DateTime resentDateOfBirth)
        {
            FileCabinetRecord updateItem = this.dateOfBirthDictionary[resentDateOfBirth].First(x => x.Id == record.Id);

            this.dateOfBirthDictionary[resentDateOfBirth].Remove(updateItem);

            if (!this.dateOfBirthDictionary.ContainsKey(newDateOfBirth))
            {
                this.dateOfBirthDictionary.Add(newDateOfBirth, new List<FileCabinetRecord>());
            }

            this.dateOfBirthDictionary[newDateOfBirth].Add(record);
        }

        public void RemoveRecord(int id)
        {
            FileCabinetRecord temp = null;

            foreach (var record in this.list)
            {
                if (record.Id == id)
                {
                    temp = record;
                    this.list.Remove(record);
                    break;
                }
            }

            if (temp != null)
            {
                this.RemoveRecordFromDictionaries(temp);
            }
        }

        private void RemoveRecordFromDictionaries(FileCabinetRecord record)
        {
            this.firstNameDictionary[record.FirstName].Remove(record);
            this.lastNameDictionary[record.LastName].Remove(record);
            this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
        }
    }
}
