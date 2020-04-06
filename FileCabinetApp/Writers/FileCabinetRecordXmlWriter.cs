using System.Xml;

namespace FileCabinetApp
{
    public class FileCabinetRecordXmlWriter
    {
        private XmlWriter xmlWriter;

        public FileCabinetRecordXmlWriter(XmlWriter xmlWriter) => this.xmlWriter = xmlWriter;

        public void Write(FileCabinetRecord record)
        {
            xmlWriter.WriteStartElement($"Employee {record.Id}");

            xmlWriter.WriteElementString("FirstName", record.FirstName);
            xmlWriter.WriteElementString("LastName", record.LastName);
            xmlWriter.WriteElementString("Salary", record.DateOfBirth.ToString());

            xmlWriter.WriteEndElement();
        }
    }
}
