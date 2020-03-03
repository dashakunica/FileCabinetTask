using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private FileStream fileStream;
        private IRecordValidator validator;

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

            record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = record.FirstName,
                LastName = record.LastName,
                DateOfBirth = record.DateOfBirth,
            };

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
            throw new NotImplementedException();
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
