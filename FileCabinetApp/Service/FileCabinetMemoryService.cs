using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp
{
    /// <summary>
    /// Service for working with file cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private const int Zero = 0;
        private const int MinId = 1;
        private const string And = "and";

        private static readonly PropertyInfo[] FileCabinetProperties = typeof(FileCabinetRecord).GetProperties();
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly IRecordValidator validator;

        private int lastId = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        /// <summary>
        /// Create service.
        /// </summary>
        /// <param name="recordValidator">Record validator type.</param>
        /// <returns>Service.</returns>
        public static IFileCabinetService Create(IRecordValidator recordValidator)
            => new FileCabinetMemoryService(recordValidator ?? throw new ArgumentNullException(nameof(recordValidator)));

        /// <inheritdoc/>
        public int CreateAndSetId(ValidateParametersData data)
        {
            return this.CreateRecord(this.GenerateId(), data);
        }

        /// <inheritdoc/>
        public int CreateRecord(int id, ValidateParametersData data)
        {
            if (id < MinId)
            {
                throw new InvalidOperationException($"Invalid id #{id} value.");
            }

            if (this.list.Any(x => x.Id == id))
            {
                throw new InvalidOperationException($"Record with id #{id} already exists.");
            }

            this.validator.ValidateParameters(data ?? throw new ArgumentNullException(nameof(data)));
            Memoization.RefreshMemoization();
            var record = DataHelper.CreateRecordFromArgs(id != 0 ? id : this.GenerateId(), data);
            this.list.Add(record);
            return record.Id;
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            var records = this.list;

            foreach (var item in records)
            {
                yield return item;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> FindRecords(FileCabinetRecord predicate, string type)
        {
            IEnumerable<FileCabinetRecord> result = this.list;

            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (predicate is null)
            {
                return this.GetRecords();
            }

            string query = predicate.ToString() + type;
            var findResult = Memoization.Saved.Find(x => x.Item1.Equals(query, StringComparison.InvariantCultureIgnoreCase));
            if (findResult != null)
            {
                return findResult.Item2;
            }

            if (type.Equals(And, StringComparison.InvariantCultureIgnoreCase))
            {
                result = this.SelectAnd(predicate, this.list);
                Memoization.Saved.Add(new Tuple<string, IEnumerable<FileCabinetRecord>>(query, result));
                return result;
            }
            else
            {
                result = this.SelectOr(predicate, this.list);
                Memoization.Saved.Add(new Tuple<string, IEnumerable<FileCabinetRecord>>(query, result));
                return result;
            }
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat() => (this.list.Count, Zero);

        /// <inheritdoc/>
        public void Purge()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void EditRecord(int id, ValidateParametersData data)
        {
            try
            {
                var current = this.list.First(x => x.Id == id);
                this.validator.ValidateParameters(data ?? throw new ArgumentNullException(nameof(data)));
                Memoization.RefreshMemoization();
                DataHelper.UpdateRecordFromData(current.Id, data, current);
            }
            catch (Exception)
            {
                throw new ArgumentException("Cannot edit record.");
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

                try
                {
                    if (this.list.Any(x => x.Id == record.Id))
                    {
                        this.EditRecord(record.Id, data);
                    }
                    else
                    {
                        this.CreateRecord(record.Id, data);
                    }
                }
                catch (ArgumentException exeption)
                {
                    snapshot.Logger.Add($"{exeption.Message}. Exeption in record #{record.Id}.");
                }
                catch (InvalidOperationException exeption)
                {
                    snapshot.Logger.Add($"{exeption.Message}. Error in record #{record.Id}.");
                }
            }
        }

        private void RemoveRecord(int id)
        {
            Memoization.RefreshMemoization();
            try
            {
                this.list.Remove(this.list.First(x => x.Id == id));
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine($"Record with id {id} does not exist.");
            }
        }

        private int GenerateId()
        {
            var start = this.lastId != int.MaxValue - 1 ? this.lastId : MinId;

            for (int i = start; i < int.MaxValue; i++)
            {
                if (!this.list.Any(x => x.Id == i))
                {
                    this.lastId = i;
                    return i;
                }
            }

            throw new IndexOutOfRangeException();
        }

        private IEnumerable<FileCabinetRecord> SelectAnd(FileCabinetRecord record, IEnumerable<FileCabinetRecord> allRecords)
        {
            var result = new List<FileCabinetRecord>(allRecords);
            if (record.FirstName != null)
            {
                result.RemoveAll(x => !record.FirstName.Equals(x.FirstName, StringComparison.InvariantCultureIgnoreCase));
            }

            if (record.LastName != null)
            {
                result.RemoveAll(x => !record.LastName.Equals(x.LastName, StringComparison.InvariantCultureIgnoreCase));
            }

            foreach (var prop in FileCabinetProperties)
            {
                Type itemType = prop.PropertyType;
                var item = prop.GetValue(record);

                if (!itemType.Equals(typeof(string)))
                {
                    if (!this.IsNullOrDefault(item))
                    {
                        result.RemoveAll(x => !item.Equals(prop.GetValue(x)));
                    }
                }
            }

            return result;
        }

        private IEnumerable<FileCabinetRecord> SelectOr(FileCabinetRecord record, IEnumerable<FileCabinetRecord> allRecords)
        {
            var result = new List<FileCabinetRecord>();
            if (record.FirstName != null)
            {
                result.AddRange(allRecords.Where(x => record.FirstName.Equals(x.FirstName, StringComparison.InvariantCultureIgnoreCase)).Where(y => !result.Contains(y)));
            }

            if (record.LastName != null)
            {
                result.AddRange(allRecords.Where(x => record.LastName.Equals(x.LastName, StringComparison.InvariantCultureIgnoreCase)).Where(y => !result.Contains(y)));
            }

            foreach (var prop in FileCabinetProperties)
            {
                Type itemType = prop.PropertyType;
                var item = prop.GetValue(record);

                if (!itemType.Equals(typeof(string)))
                {
                    if (!this.IsNullOrDefault(item))
                    {
                        result.AddRange(allRecords.Where(x => item.Equals(prop.GetValue(x))).Where(y => !result.Contains(y)));
                    }
                }
            }

            return result;
        }

        private bool IsNullOrDefault(object item)
        {
            if (item is null)
            {
                return true;
            }

            if (item.Equals(default(int))
                || item.Equals(default(DateTime))
                || item.Equals(default(char))
                || item.Equals(default(decimal))
                || item.Equals(default(short)))
            {
                return true;
            }

            return false;
        }
    }
}
