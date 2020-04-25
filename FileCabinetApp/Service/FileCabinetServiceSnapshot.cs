using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace FileCabinetApp
{
    public class FileCabinetServiceSnapshot
    {
        private FileCabinetRecord[] records;

        public FileCabinetServiceSnapshot(FileCabinetRecord[] records)
        {
            this.records = records;
        }

        public IList<string> Logger { get; } = new List<string>();

        public ReadOnlyCollection<FileCabinetRecord> FileCabinetRecords { get => new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(this.records)); }

        public void LoadFrom(IRecordReader reader)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            try
            {
                this.records = reader.ReadAll().ToArray();
            }
            catch (Exception e)
            {
                throw new ImportFailedException(e.Message);
            }
        }

        public void SaveTo(IRecordWriter writer)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            foreach (var record in this.records)
            {
                writer.Write(record);
            }
        }
    }
}
