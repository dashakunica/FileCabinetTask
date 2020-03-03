using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const long SIZEOFRECORD = 256;
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private IRecordValidator validator;
        

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
            this.validator.ValidateParameter(record);

            byte[] byteId = BitConverter.GetBytes(record.Id);
            byte[] byteFirstName = Encoding.UTF8.GetBytes(record.FirstName);
            byte[] byteLastName = Encoding.UTF8.GetBytes(record.LastName);
            byte[] byteYear = BitConverter.GetBytes(record.DateOfBirth.Year);
            byte[] byteMonth = BitConverter.GetBytes(record.DateOfBirth.Month);
            byte[] byteDay = BitConverter.GetBytes(record.DateOfBirth.Day);

            using (this.fileStream)
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(this.fileStream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(byteId);
                    binaryWriter.Write(byteFirstName);
                    binaryWriter.Write(byteLastName);
                    binaryWriter.Write(byteYear);
                    binaryWriter.Write(byteMonth);
                    binaryWriter.Write(byteDay);
                }
            }

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
                long numberOfRecords = this.fileStream.Length / SIZEOFRECORD;
                for (int i = 0; i < numberOfRecords; i++)
                {
                    record.Id = binReader.ReadInt32();
                    record.FirstName = binReader.ReadString();
                    record.LastName = binReader.ReadString();
                    int year = binReader.ReadInt32();
                    int month = binReader.ReadInt32();
                    int day = binReader.ReadInt32();

                    record.DateOfBirth = new DateTime(year, month, day);

                    list.Add(record);
                }

                ReadOnlyCollection<FileCabinetRecord> readOnlyRecords = new ReadOnlyCollection<FileCabinetRecord>(list);

                return readOnlyRecords;
            }
        }

        public int GetStat()
        {
            throw new NotImplementedException();
        }

        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
