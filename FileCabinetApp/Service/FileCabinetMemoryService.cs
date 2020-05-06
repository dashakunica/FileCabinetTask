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
                throw new InvalidOperationException($"Invalid #{id} value.");
            }

            if (this.GetRecords().Any(x => x.Id == id))
            {
                throw new InvalidOperationException($"Record #{id} doesn't exists.");
            }

            this.validator.ValidateParameters(data ?? throw new ArgumentNullException(nameof(data)));
            Memoization.RefreshMemoization();
            var record = DataHelper.CreateRecordFromArgs(id != default ? id : this.GenerateId(), data);
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
                var current = this.GetRecords().First(x => x.Id == id);
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
                    if (this.GetRecords().Any(x => x.Id == record.Id))
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

        /// <inheritdoc/>
        public void RemoveRecord(int id)
        {
            Memoization.RefreshMemoization();
            this.list.Remove(this.GetRecords().First(x => x.Id == id));
        }

        private int GenerateId()
        {
            var start = this.lastId != int.MaxValue - 1 ? this.lastId : MinId;

            for (int i = start; i < int.MaxValue; i++)
            {
                if (!this.GetRecords().Any(x => x.Id == i))
                {
                    this.lastId = i;
                    return i;
                }
            }

            throw new IndexOutOfRangeException();
        }
    }
}
