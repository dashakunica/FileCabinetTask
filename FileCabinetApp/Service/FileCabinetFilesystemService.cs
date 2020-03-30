using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Linq;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MaxStringLength = 120;
        private const long SizeOfRecord = sizeof(int) * 4 + MaxStringLength * 2;
        private const short RemovedFlag = 0b0000_0000_0000_0100;

        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private readonly BinaryReader binaryReader;
        private readonly BinaryWriter binaryWriter;
        private IRecordValidator validator;
        private string path;

        private readonly Dictionary<int, long> identificatorCache = new Dictionary<int, long>();
        private readonly Dictionary<int, long> removedCache = new Dictionary<int, long>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<DateTime, List<FileCabinetRecord>>();

        public FileCabinetFilesystemService()
        { }

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(fileStream)} cannot be null.");
            }

            this.fileStream = fileStream;
            this.binaryReader = new BinaryReader(fileStream);
            this.binaryWriter = new BinaryWriter(fileStream);
        }

        public FileCabinetFilesystemService(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException();
            }

            this.path = path;
        }

        public FileCabinetFilesystemService(IRecordValidator validator)
        {
            if (validator is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(validator)} cannot be null.");
            }

            this.validator = validator;
        }

        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.binaryReader = new BinaryReader(fileStream);
            this.binaryWriter = new BinaryWriter(fileStream);
            this.InitializeChaches();
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

        public int CreateRecord((string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            return this.CreateRecordWithSpecifiedId(null, data);
        }

        public void EditRecord(int id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
        {
            var record = new FileCabinetRecord
            {

            };

            int beginPossitin = record.Id * (int)SizeOfRecord;
            fileStream.Position = beginPossitin;

            var byteRecord = RecordToBytes(record);
            this.fileStream.Write(byteRecord, 0, byteRecord.Length);

            this.fileStream.Flush();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            var fileCabinetRecords = this.BinaryRecordsToList();

            var matchrecords = fileCabinetRecords.FindAll(
               delegate (FileCabinetRecord name)
               {
                   return name.DateOfBirth.Equals(dateOfBirth);
               });

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(matchrecords);

            return readOnlyRecords;
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            var fileCabinetRecords = this.BinaryRecordsToList();

            var matchrecords = fileCabinetRecords.FindAll(
                delegate (FileCabinetRecord name)
                {
                    return name.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase);
                });

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(matchrecords);

            return readOnlyRecords;
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            var fileCabinetRecords = BinaryRecordsToList();

            var matchrecords = fileCabinetRecords.FindAll(
                delegate (FileCabinetRecord name)
                {
                    return name.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase);
                });

            ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(matchrecords);

            return readOnlyRecords;
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
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

        private static byte[] RecordToBytes(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException(nameof(record));
            }

            var bytes = new byte[SizeOfRecord];

            byte[] byteId = BitConverter.GetBytes(record.Id);
            byte[] byteFirstName = Encoding.ASCII.GetBytes(record.FirstName);
            byte[] byteLastName = Encoding.ASCII.GetBytes(record.LastName);
            byte[] byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
            byte[] byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
            byte[] byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);

            using (var memoryStream = new MemoryStream(bytes))
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.ASCII))
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

                record.FirstName = Encoding.ASCII.GetString(nameBuffer, 0, nameLength);

                var lastNameLength = binaryReader.ReadInt32();
                var lastNameBuffer = binaryReader.ReadBytes(MaxStringLength);

                record.LastName = Encoding.ASCII.GetString(lastNameBuffer, 0, lastNameLength);

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
            long numberOfRecords = this.fileStream.Length / SizeOfRecord;
            for (int i = 0; i < numberOfRecords; i++)
            {
                var recordBuffer = new byte[SizeOfRecord];

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

        private int CreateRecordWithSpecifiedId(int? id, (string firstName, string lastName, DateTime dateOfBirth, short bonuses, decimal salary, char accountType) data)
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
                Id = id != null ? id.Value : (int)(this.fileStream.Length / SizeOfRecord) + 1,
                FirstName = data.firstName,
                LastName = data.lastName,
                DateOfBirth = data.dateOfBirth,
                Bonuses = data.bonuses,
                Salary = data.salary,
                AccountType = data.accountType,
            };

            var byteRecord = RecordToBytes(record);
            this.fileStream.Write(byteRecord, 0, byteRecord.Length);

            this.fileStream.Flush();

            return record.Id;
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
            var removed = this.GetStat().removed;
            this.fileStream.Position = 0;
            foreach (var record in records)
            {
                this.WriteToFile(this.fileStream.Position, record);
            }

            this.fileStream.SetLength(this.fileStream.Position);
            this.identificatorCache.Clear();
            this.removedCache.Clear();
            this.InitializeChaches();
            this.InitializeDictionaries();
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

        private void InitializeChaches()
        {
            var count = (int)(this.fileStream.Length / SizeOfRecord);
            for (int i = 0; i < count; i++)
            {
                var statusposition = this.FieldOfNRecord(i, FieldOffset.Status);
                this.fileStream.Position = statusposition;
                var status = this.binaryReader.ReadInt16();

                var idposition = this.FieldOfNRecord(i, FieldOffset.Id);
                this.fileStream.Position = idposition;
                var id = this.binaryReader.ReadInt32();

                if (status == RemovedFlag)
                {
                    this.removedCache.Add(id, idposition - (int)FieldOffset.Id);
                }
                else
                {
                    this.identificatorCache.Add(id, idposition - (int)FieldOffset.Id);
                }
            }
        }

        private void InitializeDictionaries()
        {
            var records = this.GetRecords();

            foreach (var record in records)
            {
                ServiceHelper.AddRecordToDictionary(record.FirstName, record, this.firstNameDictionary);
                ServiceHelper.AddRecordToDictionary(record.LastName, record, this.lastNameDictionary);
                ServiceHelper.AddRecordToDictionary(record.DateOfBirth, record, this.dateOfBirthDictionary);
            }
        }

        private long FieldOfNRecord(int number, FieldOffset fieldOffset = FieldOffset.Status)
        {
            var position = (number * SizeOfRecord) + (long)fieldOffset;
            return position < this.fileStream.Length ? position : -1;
        }
    }
}
