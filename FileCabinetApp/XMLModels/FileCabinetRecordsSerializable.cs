using System.Xml.Serialization;
using System;
using System.Collections.Generic;

namespace FileCabinetGenerator
{
    [Serializable]
    [XmlRoot("records")]
    public class FileCabinetRecordsSerializable
    {
        public FileCabinetRecordsSerializable()
        {
            this.Records = new List<FileCabinetRecordSerializable>();
        }

        public FileCabinetRecordsSerializable(IEnumerable<FileCabinetRecordSerializable> records)
        {
            this.Records = new List<FileCabinetRecordSerializable>(records);
        }

        [XmlElement("record")]
        public List<FileCabinetRecordSerializable> Records;
    }
}
