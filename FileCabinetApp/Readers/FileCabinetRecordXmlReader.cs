using FileCabinetGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");
            }

            this.reader = reader;
        }

        public IList<FileCabinetRecord> ReadAll()
        {
            var listOfRecords = new List<FileCabinetRecord>();
            var validator = new DefaultValidator();

            this.reader.BaseStream.Position = 0;

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using var xmlReader = new XmlTextReader(this.reader);
            var serializer = new XmlSerializer(typeof(FileCabinetRecordsSerializable));
            var recordRepository = (FileCabinetRecordsSerializable)serializer.Deserialize(xmlReader);

            foreach (var serializedRecord in recordRepository.Records)
            {
                FileCabinetRecord record = ConvertToRecord(serializedRecord);
                listOfRecords.Add(record);
            }

            return listOfRecords;
        }

        private static FileCabinetRecord ConvertToRecord(FileCabinetRecordSerializable item)
        {
            return new FileCabinetRecord()
            {
                Id = item.Id,
                FirstName = item.Name.FirstName,
                LastName = item.Name.LastName,
                DateOfBirth = item.DateOfBirth,
                Bonuses = item.Bonuses,
                Salary = item.Salary,
                AccountType = item.AccountType,
            };
        }
    }
}
