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

        public ReadOnlyCollection<FileCabinetRecord> Records { get => new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>(this.records)); }

        public void LoadFrom(IFileCabinetReader reader)
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
            }
        }

        public void SaveTo(IFileCabinetWriter writer)
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
