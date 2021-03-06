﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Reader from xml.
    /// </summary>
    public class FileCabinetRecordXmlReader : IFileCabinetReader
    {
        private readonly StreamReader reader;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Reader.</param>
        public FileCabinetRecordXmlReader(StreamReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");
            }

            this.reader = reader;
        }

        /// <inheritdoc/>
        public IList<FileCabinetRecord> ReadAll()
        {
            var listOfRecords = new List<FileCabinetRecord>();

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

        /// <summary>
        /// Dispose.
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
                this.reader.Dispose();
            }

            this.disposed = true;
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
