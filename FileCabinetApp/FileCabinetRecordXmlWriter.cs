using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    public class FileCabinetRecordXmlWriter
    {
        private XmlWriter xmlWriter;

        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter) => this.xmlWriter = xmlWriter;

        public void Write(FileCabinetRecord record)
        {
            this.xmlWriter.Write(record.Id);
            this.xmlWriter.Write(record.FirstName);
            this.xmlWriter.Write(record.LastName);
            this.xmlWriter.Write(record.DateOfBirth);
        }
    }
}
