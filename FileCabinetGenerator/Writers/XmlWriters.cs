using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinet.Writers
{
    [XmlRoot("records")]
    public sealed class FileCabinetRecords
    {
        [XmlElement("record")]
        public IEnumerable<FileCabinetRecordSerializable> records;
    }

    public class XmlWriters
    {
        private XmlWriter fileStream;
        FileCabinetRecords records = new FileCabinetRecords();

        public XmlWriters(XmlWriter fileStream, IEnumerable<FileCabinetRecordSerializable> records)
        {
            this.fileStream = fileStream;
            this.records.records = records;
        }

        public void Write()
        {
            XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var xmlSerializer = new XmlSerializer(typeof(FileCabinetRecords));
            xmlSerializer.Serialize(this.fileStream, this.records, emptyNamespaces);
        }
    }
}
