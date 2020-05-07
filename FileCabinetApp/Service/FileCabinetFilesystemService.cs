using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const int MaxStringLength = 120;
        private const int MinId = 0;
        private const int CharSize = sizeof(char);
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int DecimalSize = sizeof(decimal);
        private const int RecordSize = (IntSize * 4) + (MaxStringLength * 2) + CharSize + DecimalSize + ShortSize;
        private const short RemovedFlag = 0b0000_0000_0000_0100;
        private const char WhiteSpace = ' ';

        private readonly Dictionary<int, long> activeStorage = new Dictionary<int, long>();
        private readonly Dictionary<int, long> removedStorage = new Dictionary<int, long>();

        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

        private readonly FileStream fileStream;
        private readonly BinaryReader binaryReader;
        private readonly BinaryWriter binaryWriter;

        private IRecordValidator validator;
        private bool disposed = false;
        private int lastId = MinId;

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
            this.InitializeDictionaries();
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
            if (id < MinId)
            {
                throw new InvalidOperationException($"Invalid #{id} value.");
            }

            if (this.activeStorage.ContainsKey(id))
            {
                throw new InvalidOperationException($"Record #{id} doesn't exists.");
            }

            this.validator.ValidateParameters(data);
            var record = DataHelper.CreateRecordFromArgs(id, data);
            long position = this.fileStream.Length;

            if (this.removedStorage.ContainsKey(id))
            {
                position = this.removedStorage[id];
                this.removedStorage.Remove(id);
            }

            this.WriteToFileStream(position, record);
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
            this.WriteToFileStream(this.activeStorage[id], current);
        }

        /// <inheritdoc/>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            var list = this.BinaryRecordsToList();
            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(list);

            return readOnlyRecords;
        }

        /// <inheritdoc/>
        public (int active, int removed) GetStat()
        {
            return (this.activeStorage.Count, this.removedStorage.Count);
        }

        /// <inheritdoc/>
        public FileCabinetServiceSnapshot MakeSnapshot()
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
            var records = this.GetRecords();
            this.fileStream.Position = 0;
            foreach (var record in records)
            {
                this.WriteToFile(this.fileStream.Position, record);
            }

            this.fileStream.SetLength(this.fileStream.Position);
            this.activeStorage.Clear();
            this.removedStorage.Clear();
            this.InitializeDictionaries();
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

        private static FileCabinetRecord BytesToUser(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var record = new FileCabinetRecord();

            using (var memoryStream = new MemoryStream(bytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                record.Id = binaryReader.ReadInt32();

                var nameLength = binaryReader.ReadInt32();
                var nameBuffer = binaryReader.ReadBytes(MaxStringLength);

                record.FirstName = Encoding.Unicode.GetString(nameBuffer, 0, nameLength);

                var lastNameLength = binaryReader.ReadInt32();
                var lastNameBuffer = binaryReader.ReadBytes(MaxStringLength);

                record.LastName = Encoding.Unicode.GetString(lastNameBuffer, 0, lastNameLength);

                int day = binaryReader.ReadInt32();
                int month = binaryReader.ReadInt32();
                int year = binaryReader.ReadInt32();

                record.DateOfBirth = new DateTime(day, month, year);
            }

            return record;
        }

        private List<FileCabinetRecord> BinaryRecordsToList()
        {
            List<FileCabinetRecord> list = new List<FileCabinetRecord>();
            long numberOfRecords = this.fileStream.Length / RecordSize;
            for (int i = 0; i < numberOfRecords; i++)
            {
                var recordBuffer = new byte[RecordSize];

                this.fileStream.Read(recordBuffer, i * recordBuffer.Length, recordBuffer.Length);
                var record = BytesToUser(recordBuffer);

                list.Add(record);
            }

            return list;
        }

        private void WriteToFileStream(long position, FileCabinetRecord record)
        {
            this.binaryWriter.Seek((int)position, SeekOrigin.Begin);
            this.binaryWriter.Write(ShortSize);
            this.binaryWriter.Write(record.Id);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.FirstName.Concat(new string(char.MinValue, MaxStringLength - record.FirstName.Length)).ToArray()));
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.LastName.Concat(new string(char.MinValue, MaxStringLength - record.LastName.Length)).ToArray()));
            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.binaryWriter.Write(record.DateOfBirth.Year);
            this.binaryWriter.Write(record.Bonuses);
            this.binaryWriter.Write(record.Salary);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.AccountType.ToString(CultureInfo.InvariantCulture)));
        }

        private FileCabinetRecord ReadRecordFromFileStream(long position)
        {
            if (position % RecordSize != 0)
            {
                throw new ArgumentException($"{nameof(position)}");
            }

            this.binaryReader.BaseStream.Position = position;
            this.binaryReader.ReadInt16();

            return new FileCabinetRecord()
            {
                Id = this.binaryReader.ReadInt32(),
                FirstName = Encoding.Unicode.GetString(this.binaryReader.ReadBytes(MaxStringLength)).Replace(char.MinValue, WhiteSpace).Trim(),
                LastName = Encoding.Unicode.GetString(this.binaryReader.ReadBytes(MaxStringLength)).Replace(char.MinValue, WhiteSpace).Trim(),
                DateOfBirth = DateTime.Parse($"{this.binaryReader.ReadInt32()}/{this.binaryReader.ReadInt32()}/{this.binaryReader.ReadInt32()}", CultureInfo.InvariantCulture),
                Bonuses = this.binaryReader.ReadInt16(),
                Salary = this.binaryReader.ReadDecimal(),
                AccountType = Encoding.Unicode.GetString(this.binaryReader.ReadBytes(CharSize)).First(),
            };
        }

        private void WriteToFile(long position, FileCabinetRecord record)
        {
            this.binaryWriter.Seek((int)position, SeekOrigin.Begin);
            this.binaryWriter.Write((short)0);
            this.binaryWriter.Write(record.Id);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.FirstName.Concat(new string(char.MinValue, MaxStringLength - record.FirstName.Length)).ToArray()));
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.LastName.Concat(new string(char.MinValue, MaxStringLength - record.LastName.Length)).ToArray()));
            this.binaryWriter.Write(record.DateOfBirth.Month);
            this.binaryWriter.Write(record.DateOfBirth.Day);
            this.binaryWriter.Write(record.DateOfBirth.Year);
            this.binaryWriter.Write(record.Bonuses);
            this.binaryWriter.Write(record.Salary);
            this.binaryWriter.Write(Encoding.Unicode.GetBytes(record.AccountType.ToString(CultureInfo.InvariantCulture)));
        }

        private void InitializeDictionaries()
        {
            this.firstNameDictionary.Clear();
            this.lastNameDictionary.Clear();
            this.dateOfBirthDictionary.Clear();

            foreach (var item in this.activeStorage)
            {
                var record = this.ReadRecordFromFileStream(item.Value);
                this.AddToDictionary(record.FirstName, item.Value, this.firstNameDictionary);
                this.AddToDictionary(record.LastName, item.Value, this.lastNameDictionary);
                this.AddToDictionary(record.DateOfBirth, item.Value, this.dateOfBirthDictionary);
            }
        }

        private void AddToDictionary<T>(T key, long value, Dictionary<T, List<long>> dictionary)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<long>());
            }

            dictionary[key].Add(value);
        }

        private int GenerateId()
        {
            var start = this.lastId != int.MaxValue - 1 ? this.lastId : MinId;

            for (int i = start; i < int.MaxValue; i++)
            {
                if (!this.activeStorage.ContainsKey(i))
                {
                    this.lastId = i;
                    return i;
                }
            }

            throw new ArgumentException($"Does'n exist available id.");
        }
    }
}
