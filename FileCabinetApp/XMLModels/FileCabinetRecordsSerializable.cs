using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Model for serialization record.
    /// </summary>
    [Serializable]
    [XmlRoot("records")]
    public class FileCabinetRecordsSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordsSerializable"/> class.
        /// File Cabinet Records Serializable.
        /// </summary>
        public FileCabinetRecordsSerializable()
        {
            this.Records = new List<FileCabinetRecordSerializable>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordsSerializable"/> class.
        /// </summary>
        /// <param name="fileCabinetRecords">Records.</param>
        public FileCabinetRecordsSerializable(IEnumerable<FileCabinetRecordSerializable> fileCabinetRecords)
        {
            this.Records = new List<FileCabinetRecordSerializable>(fileCabinetRecords);
        }

        /// <summary>
        /// Gets serialization records.
        /// </summary>
        /// <value>
        /// Records.
        /// </value>
        public List<FileCabinetRecordSerializable> Records { get; }
    }
}
