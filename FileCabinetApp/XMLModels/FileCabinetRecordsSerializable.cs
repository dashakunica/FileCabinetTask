using System.Xml.Serialization;

namespace FileCabinetGenerator
{
    [XmlRoot("records")]
    public class FileCabinetRecordsSerializable
    {
        [XmlElement("record")]
        public FileCabinetRecordSerializable[] Records { get; set; }
    }
}
