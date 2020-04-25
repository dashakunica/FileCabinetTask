using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MaxStringLength = 120;
        private const int CharSize = sizeof(char);
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int DecimalSize = sizeof(decimal);
        private const int RecordSize = (IntSize * 4) + (MaxStringLength * 2) + CharSize + DecimalSize + ShortSize;
        private const short RemovedFlag = 0b0000_0000_0000_0100;
        private const char WhiteSpace = ' ';

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private readonly BinaryReader binaryReader;
        private readonly BinaryWriter binaryWriter;
        private IRecordValidator validator;
        private string path;
        private int lastId = 0;

        private readonly Dictionary<int, long> idStorage = new Dictionary<int, long>();
        private readonly Dictionary<int, long> removedStorage = new Dictionary<int, long>();

        private readonly Dictionary<string, List<long>> firstNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<string, List<long>> lastNameDictionary = new Dictionary<string, List<long>>();
        private readonly Dictionary<DateTime, List<long>> dateOfBirthDictionary = new Dictionary<DateTime, List<long>>();

        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.binaryReader = new BinaryReader(fileStream);
            this.binaryWriter = new BinaryWriter(fileStream);
            this.InitializeDictionaries();
        }

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

        public int CreateRecord(ValidateParametersData data)
        {
            this.lastId++;
            int id = this.lastId;
            return this.CreateRecordWithId(id, data);
        }

        public void EditRecord(int id, ValidateParametersData data)
        {
            if (!this.ExistRecord(id))
            {
                throw new InvalidOperationException($"Record #{id} doesn't exists.");
            }

            var record = DataHelper.CreateRecordFromData(id, data);
            this.validator.ValidateParameters(record);
            this.WriteToFileStream(this.identificatorCache[id], record);
        }

        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            var list = this.BinaryRecordsToList();
            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(list);

            return readOnlyRecords;
        }

        public (int active, int removed) GetStat()
        {
            return (this.identificatorCache.Count, this.removedCache.Count);
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
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

        private static byte[] RecordToBytes(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var bytes = new byte[RecordSize];

            byte[] byteId = BitConverter.GetBytes(record.Id);
            byte[] byteFirstName = Encoding.Unicode.GetBytes(record.FirstName);
            byte[] byteLastName = Encoding.Unicode.GetBytes(record.LastName);
            byte[] byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
            byte[] byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
            byte[] byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);
            byte[] byteSalary = BitConverter.GetBytes(Convert.ToDouble(record.Salary));
            byte[] byteBonuses = BitConverter.GetBytes(record.Bonuses);
            byte[] byteAccountType = BitConverter.GetBytes(record.AccountType);

            using (var memoryStream = new MemoryStream(bytes))
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.Unicode))
            {
                binaryWriter.BaseStream.Position = 0;
                binaryWriter.Write(byteId);

                var nameBuffer = new byte[MaxStringLength];
                int nameLength = byteFirstName.Length;
                if (nameLength > MaxStringLength)
                {
                    nameLength = MaxStringLength;
                }

                Array.Copy(byteFirstName, 0, nameBuffer, 0, nameLength);

                binaryWriter.Write(nameLength);
                binaryWriter.Write(nameBuffer);

                var lastNameBuffer = new byte[MaxStringLength];
                int lastNameLength = byteLastName.Length;
                if (nameLength > MaxStringLength)
                {
                    nameLength = MaxStringLength;
                }

                Array.Copy(byteLastName, 0, lastNameBuffer, 0, nameLength);

                binaryWriter.Write(lastNameLength);
                binaryWriter.Write(lastNameBuffer);

                binaryWriter.BaseStream.Position = 124;
                binaryWriter.Write(byteLastName);

                binaryWriter.BaseStream.Position = 244;
                binaryWriter.Write(byteYear);

                binaryWriter.BaseStream.Position = 248;
                binaryWriter.Write(byteMonth);

                binaryWriter.BaseStream.Position = 252;
                binaryWriter.Write(byteDay);

                binaryWriter.BaseStream.Position = 258;
                binaryWriter.Write(byteSalary);

                binaryWriter.BaseStream.Position = 262;
                binaryWriter.Write(byteSalary);
            }

            return bytes;
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

        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is null)
            {
                throw new ArgumentNullException($"{nameof(snapshot)} cannot be null");
            }

            foreach (var record in snapshot.Records)
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
        }

        public int CreateRecordWithId(int id, ValidateParametersData data)
        {
            var record = DataHelper.CreateRecordFromData(id, data);

            this.validator.ValidateParameters(record);
            var position = this.fileStream.Length;
            this.WriteToFileStream(position, record);

            this.AddToDictionary(record.FirstName, position, this.firstNameDictionary);
            this.AddToDictionary(record.LastName, position, this.lastNameDictionary);
            this.AddToDictionary(record.DateOfBirth, position, this.dateOfBirthDictionary);

            var byteRecord = RecordToBytes(record);

            this.fileStream.Flush();

            return record.Id;
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

        public void RemoveRecord(int id)
        {
            if (!this.ExistRecord(id))
            {
                throw new InvalidOperationException($"Record #{id} doesn't exists.");
            }

            var removedPoition = this.MarkRecordAsRemoved(id);
            this.removedCache.Add(id, removedPoition);
            this.identificatorCache.Remove(id);
        }

        private long MarkRecordAsRemoved(int id)
        {
            var position = this.identificatorCache[id];
            this.binaryReader.BaseStream.Position = position;
            this.binaryWriter.Write(RemovedFlag);
            return position;
        }

        private bool ExistRecord(int id)
        {
            if (this.identificatorCache.ContainsKey(id))
            {
                return true;
            }

            return false;
        }

        public void Purge()
        {
            var records = this.GetRecords();
            this.fileStream.Position = 0;
            foreach (var record in records)
            {
                this.WriteToFile(this.fileStream.Position, record);
            }

            this.fileStream.SetLength(this.fileStream.Position);
            this.identificatorCache.Clear();
            this.removedCache.Clear();
            this.InitializeDictionaries();
        }

        private FileCabinetRecord ReadRecordFromFileStream(long position)
        {
            if (position % RecordSize != 0)
            {
                throw new ArgumentException(nameof(position));
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

            foreach (var item in this.identificatorCache)
            {
                var record = this.ReadRecordFromFileStream(item.Value);
                this.AddToDictionary(record.FirstName, item.Value, this.firstNameDictionary);
                this.AddToDictionary(record.LastName, item.Value, this.lastNameDictionary);
                this.AddToDictionary(record.DateOfBirth, item.Value, this.dateOfBirthDictionary);
            }
        }

        private long FieldOfNRecord(int number, FieldOffset fieldOffset = FieldOffset.Status)
        {
            var position = (number * RecordSize) + (long)fieldOffset;
            return position < this.fileStream.Length ? position : -1;
        }

        private void AddToDictionary<T>(T key, long value, Dictionary<T, List<long>> dictionary)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, new List<long>());
            }

            dictionary[key].Add(value);
        }

        private void RemoveFromDictionary<T>(T key, long value, Dictionary<T, List<long>> dictionary)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Remove(value);
            }
        }

        private int GenerateId()
        {
            var start = this.lastAssigned != int.MaxValue - 1 ? this.lastAssigned : MinIdentificator;

            for (int i = start; i < int.MaxValue; i++)
            {
                if (!this.identificatorValideCache.ContainsKey(i))
                {
                    this.lastAssigned = i;
                    return i;
                }
            }

            throw new ArgumentException($"Does'n exist available id.");
        }
    }
}
