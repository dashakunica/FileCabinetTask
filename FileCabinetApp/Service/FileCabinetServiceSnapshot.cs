using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        public FileCabinetServiceSnapshot(FileCabinetRecord[] records) => this.records = records;

        public FileCabinetRecord[] Records
        {
            get
            {
                if (this.records is null)
                {
                    throw new ArgumentNullException(nameof(records));
                }

                return records;
            }
            set => this.records = value;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return (FileCabinetRecord[])this.records.Clone();
        }

        public void SaveToCsv(StreamWriter streamWriter)
        {
            TextWriter textWriter = streamWriter;

            textWriter.Write("Id,First Name,Last Name,Date of Birth");
            FileCabinetRecordCsvWriter fileCabinetRecordCsvWriter = new FileCabinetRecordCsvWriter(textWriter);

            foreach (var record in this.records)
            {
                fileCabinetRecordCsvWriter.Write(record);
            }
        }

        public void SaveToXml(StreamWriter streamWriter)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter))
            {
                FileCabinetRecordXmlWriter fileCabinetRecordXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Records");

                foreach (var record in this.records)
                {
                    fileCabinetRecordXmlWriter.Write(record);
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        public void LoadFromCsv(StreamReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException($"{nameof(reader)} cannot be null.");
            }

            var csvReader = new FileCabinetRecordCsvReader(reader);

            try
            {
                this.records = csvReader.ReadAll().ToArray();
            }
            catch (InvalidOperationException ioe)
            {
                throw new ImportFailedException("Import failed.", ioe);
            }
        }
    }
}
