using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int MaxStringLength = 60;
        private const long SizeOfRecord = sizeof(int) * 4 + MaxStringLength * 4;
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
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            FileCabinetRecord record = new FileCabinetRecord();

                using (BinaryReader binReader = new BinaryReader(this.fileStream))
                {
                    List<FileCabinetRecord> list = new List<FileCabinetRecord>();
                    long numberOfRecords = this.fileStream.Length / SizeOfRecord;
                    for (int i = 0; i < numberOfRecords; i++)
                    {
                        binReader.BaseStream.Position = 0;
                        record.Id = binReader.ReadInt32();

                        binReader.BaseStream.Position = 4;
                        record.FirstName = binReader.ReadString();

                        binReader.BaseStream.Position = 124;
                        record.LastName = binReader.ReadString();

                        binReader.BaseStream.Position = 244;
                        int year = binReader.ReadInt32();

                        binReader.BaseStream.Position = 248;
                        int month = binReader.ReadInt32();

                        binReader.BaseStream.Position = 252;
                        int day = binReader.ReadInt32();

                        record.DateOfBirth = new DateTime(year, month, day);

                        list.Add(record);
                    }

                    ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(list);

                    return readOnlyRecords;
                }
        }

        public static byte[] RecordToBytes(FileCabinetRecord record)
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

                binaryWriter.BaseStream.Position = 4;
                binaryWriter.Write(byteFirstName);

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
    }
}
