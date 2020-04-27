using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with file cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private const int Zero = 0;
        private const int MinId = 1;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IRecordValidator validator;

        private readonly Dictionary<string, List<FileCabinetRecord>> memoization = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public FileCabinetMemoryService(IRecordValidator validator) => this.validator = validator ?? throw new ArgumentNullException(nameof(validator));

        public static IFileCabinetService Create(IRecordValidator recordValidator)
            => new FileCabinetMemoryService(recordValidator ?? throw new ArgumentNullException(nameof(recordValidator)));

        /// <summary>
        /// Method which create new record.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <returns>Id of record.</returns>
        public int CreateRecord(ValidateParametersData data)
        {
            return this.CreateRecordWithId(Zero, data);
        }

        /// <summary>
        /// Get all records.
        /// </summary>
        /// <returns>All records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            var records = this.list;

            foreach (var item in records)
            {
                yield return item;
            }
        }

        public IEnumerable<FileCabinetRecord> FindRecordsByPredicate(string query, List<FileCabinetRecord> predicate)
        {
            List<FileCabinetRecord> records = null;

            if (!this.memoization.TryGetValue(query, out records))
            {
                this.memoization.Add(query, predicate);
                records = predicate;
            }

            foreach (var item in records)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Get amount of record.
        /// </summary>
        /// <returns>Amount of records.</returns>
        public (int active, int removed) GetStat() => (this.list.Count, Zero);

        public void Purge()
        {
            throw new NotImplementedException();
        }

        public void Delete(IEnumerable<FileCabinetRecord> records)
        {
            if (records is null)
            {
                throw new ArgumentNullException(nameof(records));
            }

            foreach (var record in records)
            {
                this.RemoveRecord(record.Id);
            }
        }

        public void EditRecord(int id, ValidateParametersData data)
        {
            try
            {
                var current = this.GetRecords().First(x => x.Id == id);
                this.validator.ValidateParameters(data ?? throw new ArgumentNullException(nameof(data)));
                this.memoization.Clear();
                DataHelper.UpdateRecordFromData(current.Id, data, current);
            }
            catch (Exception)
            {
                throw new ArgumentException(nameof(id));
            }
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

        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            snapshot.Logger.Clear();

            foreach (var record in snapshot.Records)
            {
                var data = DataHelper.CreateValidateData(record);

                if (this.GetRecords().Any(x => x.Id == record.Id))
                {
                    this.EditRecord(record.Id, data);
                }
                else
                {
                    this.CreateRecordWithId(record.Id, data);
                }
            }
        }

        public int CreateRecordWithId(int id, ValidateParametersData data)
        {
            if (id < MinId)
            {
                throw new ArgumentException();
            }

            if (this.GetRecords().Any(x => x.Id == id))
            {
                throw new ArgumentException();
            }

            this.validator.ValidateParameters(data ?? throw new ArgumentException(nameof(data)));
            this.memoization.Clear();
            var record = DataHelper.CreateRecordFromArgs(id != default ? id : this.GenerateId(), data);
            this.list.Add(record);
            return record.Id;
        }

        public void RemoveRecord(int id)
        {
            this.memoization.Clear();
            this.list.Remove(this.GetRecords().First(x => x.Id == id));
        }

        private int GenerateId()
        {
            for (int i = MinId; i <= int.MaxValue; i++)
            {
                if (this.list.Count == 0)
                {
                    return MinId;
                }

                foreach (var record in this.GetRecords())
                {
                    if (record.Id == i)
                    {
                        return i;
                    }
                }
            }

            throw new IndexOutOfRangeException();
        }
    }
}
