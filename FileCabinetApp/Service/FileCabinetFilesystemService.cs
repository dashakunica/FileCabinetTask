using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// File cabinet filesystem service.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService, IDisposable
    {
        private const int StringLengthSize = 2;
        private const int CharSize = sizeof(char);
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int DecimalSize = sizeof(decimal);
        private const short RemovedFlag = 0b0000_0000_0000_0001;
        private const char WhiteSpace = ' ';

        private static readonly int MaxFNameLength = ValidatorBuilder.FNameValidValue.Max;
        private static readonly int MaxLNameLength = ValidatorBuilder.LNameValidValue.Max;

        private static readonly int RecordSize = (IntSize * 4) + ((MaxFNameLength + MaxLNameLength) * StringLengthSize)
                                                    + CharSize + DecimalSize + ShortSize + StringLengthSize;

        private readonly Dictionary<int, long> activeStorage = new Dictionary<int, long>();
        private readonly Dictionary<int, long> removedStorage = new Dictionary<int, long>();

        private readonly FileStream fileStream;
        private readonly BinaryReader binaryReader;
        private readonly BinaryWriter binaryWriter;

        private IRecordValidator validator;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="validator">Record validator type.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.binaryReader = new BinaryReader(fileStream);
            this.binaryWriter = new BinaryWriter(fileStream);
            this.SetStorage();
        }

        /// <summary>
        /// Create new service.
        /// </summary>
        /// <param name="fileStream">File stream.</param>
        /// <param name="validator">Validator type.</param>
        /// <returns>Service.</returns>
        public static IFileCabinetService Create(FileStream fileStream, IRecordValidator validator)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (validator is null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            return new FileCabinetFilesystemService(fileStream, validator);
        }

        /// <inheritdoc/>
        public int CreateAndSetId(ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return this.CreateRecord(this.GenerateId(), data);
        }

        /// <inheritdoc/>
        public int CreateRecord(int id, ValidateParametersData data)
        {
            if (this.activeStorage.ContainsKey(id))
            {
                throw new InvalidOperationException($"Record with #{id} already exists.");
            }

            this.validator.ValidateParameters(data);
            var record = DataHelper.CreateRecordFromArgs(id, data);
            long position = this.fileStream.Length;

            if (this.removedStorage.ContainsKey(id))
            {
                position = this.removedStorage[id];
                this.removedStorage.Remove(id);
            }

            this.RecordsToBytes(position, record);
            this.activeStorage.Add(id, position);
            return record.Id;
        }

        /// <inheritdoc/>
        public void EditRecord(int id, ValidateParametersData data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!this.activeStorage.ContainsKey(id))
            {
                throw new InvalidOperationException($"Record #{id} doesn't exists.");
            }

            this.validator.ValidateParameters(data);
            var current = DataHelper.CreateRecordFromArgs(id, data);
            DataHelper.UpdateRecordFromData(current.Id, data, current);
            this.RecordsToBytes(this.activeStorage[id], current);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            foreach (var item in this.activeStorage.Values.ToList())
            {
                var record = this.BytesToRecord(item);

                yield return record;
            }
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            return (this.activeStorage.Count, this.removedStorage.Count);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords().ToArray());
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
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} cannot be null");
            }

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
            if (!this.activeStorage.ContainsKey(id))
            {
                throw new InvalidOperationException($"Record #{id} doesn't exists.");
            }

            var removedPoition = this.activeStorage[id];
            this.binaryReader.BaseStream.Position = removedPoition;
            this.binaryWriter.Write(RemovedFlag);

            this.removedStorage.Add(id, removedPoition);
            this.activeStorage.Remove(id);
        }

        /// <inheritdoc/>
        public void Purge()
        {
            var currentPosition = 0;
            var startRecordPositions = new List<long>(this.activeStorage.Values);
            startRecordPositions.Sort();

            foreach (var start in startRecordPositions)
            {
                this.RecordsToBytes(currentPosition, this.BytesToRecord(start));
                currentPosition += RecordSize;
            }

            this.fileStream.SetLength(currentPosition);

            this.SetStorage();
        }

        /// <summary>
        /// Dispose interface realization.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">True or false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.binaryReader.Close();
                this.binaryWriter.Close();
            }

            this.disposed = true;
        }

        private void RecordsToBytes(long position, FileCabinetRecord record)
        {
            this.binaryWriter.Seek((int)position, SeekOrigin.Begin);
            this.binaryWriter.Write((short)0);
            this.binaryWriter.Write(record.Id);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.FirstName.Concat(new string(WhiteSpace, 60 - record.FirstName.Length)).ToArray()));
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.LastName.Concat(new string(WhiteSpace, 60 - record.LastName.Length)).ToArray()));
            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.binaryWriter.Write(record.DateOfBirth.Year);
            this.binaryWriter.Write(record.Bonuses);
            this.binaryWriter.Write(record.Salary);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.AccountType.ToString(CultureInfo.InvariantCulture)));
        }

        private FileCabinetRecord BytesToRecord(long position)
        {
            if (position % RecordSize != 0)
            {
                throw new ArgumentException($"{nameof(position)}");
            }

            this.binaryReader.BaseStream.Position = position;
            this.binaryReader.ReadInt16();

            var record = new FileCabinetRecord()
            {
                Id = this.binaryReader.ReadInt32(),
                FirstName = Encoding.Unicode.GetString(this.binaryReader.ReadBytes(60 * 2)).Trim(),
                LastName = Encoding.Unicode.GetString(this.binaryReader.ReadBytes(60 * 2)).Trim(),
                DateOfBirth = DateTime.Parse($"{this.binaryReader.ReadInt32()}/{this.binaryReader.ReadInt32()}/{this.binaryReader.ReadInt32()}", CultureInfo.InvariantCulture),
                Bonuses = this.binaryReader.ReadInt16(),
                Salary = this.binaryReader.ReadDecimal(),
                AccountType = Encoding.Unicode.GetString(this.binaryReader.ReadBytes(CharSize)).First(),
            };

            return record;
        }

        private void WriteToFile(long position, FileCabinetRecord record)
        {
            this.binaryWriter.Seek((int)position, SeekOrigin.Begin);
            this.binaryWriter.Write((short)0);
            this.binaryWriter.Write(record.Id);

            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.FirstName.Concat(
                new string(char.MinValue, (MaxFNameLength * StringLengthSize) - record.FirstName.Length)).ToArray()));
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.LastName.Concat(
                new string(char.MinValue, (MaxLNameLength * StringLengthSize) - record.LastName.Length)).ToArray()));

            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.binaryWriter.Write(record.DateOfBirth.Year);

            this.binaryWriter.Write(record.Bonuses);
            this.binaryWriter.Write(record.Salary);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.AccountType.ToString(CultureInfo.InvariantCulture)));
        }

        private int GenerateId()
        {
            for (int i = 1; i < int.MaxValue; i++)
            {
                if (!this.activeStorage.ContainsKey(i))
                {
                    return i;
                }
            }

            throw new ArgumentException($"Does'n exist available id.");
        }

        private void SetStorage()
        {
            this.activeStorage.Clear();
            this.removedStorage.Clear();

            var count = (int)(this.fileStream.Length / RecordSize);
            for (int i = 0; i < count; i++)
            {
                this.fileStream.Position = i * RecordSize;
                var status = this.binaryReader.ReadInt16();

                var idposition = (i * RecordSize) + 2;
                this.fileStream.Position = idposition;
                var id = this.binaryReader.ReadInt32();

                if (status == RemovedFlag)
                {
                    this.removedStorage.Add(id, idposition - 2);
                }
                else
                {
                    var beginOfTheRecord = idposition - 2;
                    if (this.IsValid(beginOfTheRecord))
                    {
                        this.activeStorage.Add(id, beginOfTheRecord);
                    }
                }
            }
        }

        private bool IsValid(long beginOfTheRecord)
        {
            var record = this.BytesToRecord(beginOfTheRecord);
            var validate = DataHelper.CreateValidateData(record);

            try
            {
                this.validator.ValidateParameters(validate);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
