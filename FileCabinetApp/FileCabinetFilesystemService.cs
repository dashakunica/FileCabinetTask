using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Linq;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MaxStringLength = 120;
        private const long SizeOfRecord = sizeof(int) * 4 + MaxStringLength * 2;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private IRecordValidator validator;
        private string path;

        public FileCabinetFilesystemService()
        { }

        public FileCabinetFilesystemService(FileStream fileStream)
        {
            if (fileStream is null)
            {
                throw new ArgumentNullException($"Parameter {nameof(fileStream)} cannot be null.");
            }

            this.fileStream = fileStream;
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

        public int CreateRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException();
            }

            var byteRecord = RecordToBytes(record);
            this.fileStream.Write(byteRecord, 0, byteRecord.Length);

            this.fileStream.Flush();

            return record.Id;
        }

        public void EditRecord(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException();
            }

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

        public int GetStat()
        {
            if (this.fileStream is null)
            {
                return 0;
            }

            long numberOfRecords = this.fileStream.Length / SizeOfRecord;
            return (int)numberOfRecords;
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
    }
}
