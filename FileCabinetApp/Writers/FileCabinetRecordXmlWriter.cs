using System;
using System.IO;
using System.Xml;
using System.Globalization;

namespace FileCabinetApp
{
    public class FileCabinetRecordXmlWriter : IFileCabinetWriter
    {
        private readonly XmlWriter xmlTextWriter;
        private bool disposed = false;

        public FileCabinetRecordXmlWriter(StreamWriter writer)
        {
            this.xmlTextWriter = new XmlTextWriter(writer ?? throw new ArgumentNullException(nameof(writer)));
            this.xmlTextWriter.WriteStartDocument();
            this.xmlTextWriter.WriteStartElement("records");
        }

        public void Write(FileCabinetRecord record)
        {
            if (record is null)
            {
                throw new ArgumentNullException($"{nameof(record)} cannot be null.");
            }

            this.xmlTextWriter.WriteStartElement("record");
            this.xmlTextWriter.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));
            this.xmlTextWriter.WriteStartElement("name");
            this.xmlTextWriter.WriteAttributeString("first", record.FirstName);
            this.xmlTextWriter.WriteAttributeString("last", record.LastName);
            this.xmlTextWriter.WriteEndElement();
            this.xmlTextWriter.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("MM/dd/yy", CultureInfo.InvariantCulture));
            this.xmlTextWriter.WriteElementString("bonuses", record.Bonuses.ToString(CultureInfo.InvariantCulture));
            this.xmlTextWriter.WriteElementString("salary", record.Salary.ToString(CultureInfo.InvariantCulture));
            this.xmlTextWriter.WriteElementString("accountType", record.AccountType.ToString(CultureInfo.InvariantCulture));
            this.xmlTextWriter.WriteEndElement();
        }
    }
}
