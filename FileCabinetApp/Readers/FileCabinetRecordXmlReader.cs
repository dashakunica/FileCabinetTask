using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetGenerator;

namespace FileCabinetApp
{
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        /// <summary>Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.</summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="ArgumentNullException">Argument cannot be null.</exception>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");
            }

            this.reader = reader;
        }

        /// <summary>Reads all.</summary>
        /// <returns>List of records.</returns>
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
